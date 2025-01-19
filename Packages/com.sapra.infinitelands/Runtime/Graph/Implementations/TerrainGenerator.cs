using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using System.IO;
using sapra.InfiniteLands.NaughtyAttributes;

namespace sapra.InfiniteLands
{
    public abstract class TerrainGenerator : ScriptableObject, IGraph
    {     
        [HideInInspector] public bool AutoUpdate;

        public bool _autoUpdate => AutoUpdate;

        public Vector2 position;
        public Vector2 scale = Vector2.one;

        public bool ValidOutput{get; private set;}

        public Vector2 MinMaxHeight
        {
            get { 
                if(output.HeightMap != null)
                    return output.HeightMap.minMaxValue; 
                return Vector2.one;
            }
        }

        public HeightOutputNode output;
        public List<InfiniteLandsNode> nodes = new List<InfiniteLandsNode>();
        public List<GroupBlock> groups = new List<GroupBlock>();
        public List<StickyNoteBlock> stickyNotes = new List<StickyNoteBlock>();

        public Action OnValuesChanged{get; set;} = default;
        
        #region Initalization
        public virtual void InitializingAsset(){}
        
        private void OnEnable(){
            ValidateThatItHasOutput();
        }

        public void ValidateThatItHasOutput(){
            #if UNITY_EDITOR
            if(output == null && EditorUtility.IsPersistent(this)){
                output = CreateNode(typeof(HeightOutputNode), new Vector2(200, 0)) as HeightOutputNode;
            }
            #endif
        }

        public IEnumerable<InfiniteLandsNode> GetNodes() => nodes;
        public HeightOutputNode GetOutputNode() => output;

        public void Initialize()
        {
            OnValuesChanged -= ValidationCheck;
            OnValuesChanged += ValidationCheck;

            InitializingAsset();
        }
        public void ValidationCheck(){
            nodes.RemoveAll(a => a == null);

            if(output == null){
                ValidOutput = false;
                return;
            }
                
            var ls = nodes.Where(a => typeof(IOutput).IsAssignableFrom(a.GetType()));
            foreach(InfiniteLandsNode nd in ls){
                nd.ValidationCheck();
                if(!nd.isValid){
                    ValidOutput = false;
                    return;
                }
            }
            ValidOutput = true;
        }
        
        public IEnumerable<AssetWithType> GetAssets(){
            return nodes.
                Where(a => a.isValid).
                OfType<ILoadAsset>().
                SelectMany(a => a.GetAssets()).
                GroupBy(a => a.GetType(), b => b, (key, value) => new AssetWithType(){
                    originalType = key,
                    assets = value.Where(a => a != null).Distinct()}).
                Where(a => a.originalType != null).ToArray();
        }

        
        public Dictionary<IAsset, ILoadAsset[]> GetAssetLoaders()
        {
            var allAssets = GetAssets().SelectMany(a => a.assets);
            Dictionary<IAsset, ILoadAsset[]> result = new();
            foreach(IAsset asset in allAssets){
                result.Add(asset, GetAssetNodesSameAs(asset).ToArray());
            }
            return result;
        }

        public IEnumerable<ILoadAsset> GetAssetNodes<M>() where M : IAsset => nodes.Where(a => a.isValid).OfType<ILoadAsset>();
        public IEnumerable<ILoadAsset> GetAssetNodesSameAs<M>(M asset) where M : IAsset => GetAssetNodes<M>().Where(a => a.AssetExists(asset));
        #endregion

        #if UNITY_EDITOR

        #region Nodes Configuration
        public InfiniteLandsNode CreateNodeFromJson(Type type, string JsonData, Vector2 position){
            if(!typeof(InfiniteLandsNode).IsAssignableFrom(type))
                return null;
            
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            InfiniteLandsNode result = CreateInstance(type) as InfiniteLandsNode;
            Undo.RegisterCreatedObjectUndo(result, "Created Node from JSON");
            Undo.RegisterCompleteObjectUndo(result, "Modify Node");
            
            JsonUtility.FromJsonOverwrite(JsonData, result);
            ConfigureAsset(result, position);
            
            Undo.SetCurrentGroupName(string.Format("{0}: Creating Node", this.name));
            Undo.CollapseUndoOperations(curGroupID);
            return result;
        }
        public InfiniteLandsNode CreateNode(Type type, Vector2 position)
        {  
            if(typeof(InfiniteLandsNode).IsEquivalentTo(type))
                return null;
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            InfiniteLandsNode node = CreateInstance(type) as InfiniteLandsNode;
            Undo.RegisterCreatedObjectUndo(node, "Created Node");
            ConfigureAsset(node, position);

            Undo.SetCurrentGroupName(string.Format("{0}: Creating {1}", this.name, type.Name));
            Undo.CollapseUndoOperations(curGroupID);
            return node;
        }

        public void Remove(InfiniteLandsNode node)
        {
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();
            
            node.OnValuesUpdated = null;

            Record("Remove Node");
            DeleteNodeFromConnections(node);
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            
            Undo.SetCurrentGroupName(string.Format("{0}: Removing {1}", this.name, node.GetType().Name));
            Undo.CollapseUndoOperations(curGroupID);
        }

        public void DeleteNodeFromConnections(InfiniteLandsNode oldNode){
            foreach(InfiniteLandsNode node in nodes){
                List<Connection> connections = new List<Connection>(node.GetConnections());
                foreach(Connection connection in connections){
                    if(connection.provider.Equals(oldNode)){
                        Undo.RecordObject(node, "Removing a connection");
                        node.RemoveConnection(connection);
                    }
                }
            }
        }

        public bool Connect(InfiniteLandsNode node, string inputPortName, 
            InfiniteLandsNode provider, string providerPortName)
        {
            Connection connection = new Connection()
            {
                provider = provider,
                providerPortName = providerPortName,
                inputPortName = inputPortName,
            };
            Undo.RecordObject(node, string.Format("{0}: {1}", node.name, "Added Connection"));
            bool result = node.AddConnection(connection);
            return result;
        }

        public void RemoveConnection(InfiniteLandsNode node, string inputPortName, InfiniteLandsNode provider,
            string providerPortName)
        {
            Undo.RecordObject(node, string.Format("{0}: {1}", node.name, "Removed Connection"));
            node.RemoveConnection(provider, providerPortName, inputPortName);
        }

        #endregion

        #region Groups
        
        public GroupBlock CreateGroupFromJson(string JsonData, Vector2 position){            
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            GroupBlock result = new GroupBlock();
            JsonUtility.FromJsonOverwrite(JsonData, result);
            ConfigureAsset(result, position);
            return result;
        }
        public GroupBlock CreateGroup(string name, Vector2 position, List<string> elementsGUIDS){
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            GroupBlock block = new GroupBlock
            {
                Name = name,
                elementGuids = elementsGUIDS,
            };
            ConfigureAsset(block, position);

            Undo.SetCurrentGroupName(string.Format("{0}: Creating group", this.name));
            Undo.CollapseUndoOperations(curGroupID);
            return block;
        }

        public void Remove(GroupBlock group){
            Record("Removed Group");
            groups.Remove(group);
        }

        public void AddElementsToGroup(GroupBlock group, IEnumerable<string> guids)
        {
            Record("Added item to Group");
            group.elementGuids.AddRange(guids);
        }

        public void RemoveElementsToGroup(GroupBlock group, IEnumerable<string> guids)
        {
            Record("Removed item from group");
            foreach(string guid in guids){
                group.elementGuids.Remove(guid);
            }
        }

        #endregion

        #region StickyNotes
        public StickyNoteBlock CreateStickyNoteFromJson(string JsonData, Vector2 position){
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            StickyNoteBlock result = new StickyNoteBlock();
            JsonUtility.FromJsonOverwrite(JsonData, result);
            ConfigureAsset(result, position);

            Undo.SetCurrentGroupName(string.Format("{0}: Creating Sticky Node", this.name));
            Undo.CollapseUndoOperations(curGroupID);
            return result;
        }
        public StickyNoteBlock CreateStickyNote(Vector2 position){
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            StickyNoteBlock sn = new StickyNoteBlock(){
                title = "New note",
                content = "Add content here",
            };      
            ConfigureAsset(sn, position);
                         
            Undo.SetCurrentGroupName(string.Format("{0}: Creating Sticky Node", this.name));
            Undo.CollapseUndoOperations(curGroupID);
            return sn;
        }

        public void Remove(StickyNoteBlock stickyNote){
            Record("Remove Sticky Note");
            stickyNotes.Remove(stickyNote);
        }
        #endregion

        #region Configure
        private void ConfigureAsset(InfiniteLandsNode node, Vector2 position){
            Undo.RecordObject(node, "Configuring it");
            node.guid = GUID.Generate().ToString();
            node.position = position;
            node.name = node.GetType().Name;
            
            Record("Added Node "+ node.GetType().Name);
            nodes.Add(node);
            EditorUtility.SetDirty(node);
            AssetDatabase.AddObjectToAsset(node, this);
        }
        
        private void ConfigureAsset(StickyNoteBlock note, Vector2 position){
            note.guid = GUID.Generate().ToString();
            note.position = position;

            Record("Added Sticky Note");
            stickyNotes.Add(note);
        }

        private void ConfigureAsset(GroupBlock block, Vector2 position){
            block.guid = GUID.Generate().ToString();
            block.position = position;

            Record("Added Group");
            groups.Add(block);
        }
        #endregion
        
        private void Record(string action){
            Undo.RecordObject(this, string.Format("{0}: {1}", this.name, action));
            EditorUtility.SetDirty(this);
        }

		[MenuItem("Assets/Create/InfiniteLands/Simple World", priority = 102)]
        public static void CreateDefaultWorld(){
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                CreateInstance<OnceCreated>(), 
                "Infinite Lands World.asset", EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D, null);
        }

        class OnceCreated : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                WorldTree world = CreateInstance<WorldTree>();
                world.name = Path.GetFileName(pathName);
                AssetDatabase.CreateAsset(world, pathName);
                InfiniteLandsNode output = world.CreateNode(typeof(HeightOutputNode), Vector2.zero);
                InfiniteLandsNode node = world.CreateNode(typeof(SimplexNoiseNode), new Vector2(-235,0));
                SimplexNoiseNode nd = node as SimplexNoiseNode;
                nd.TileSize = 200;
                nd.MinMaxHeight = new Vector2(0,250);
                nd.Octaves = 5;
                world.output = output as HeightOutputNode; 
                world.Connect(world.output, nameof(world.output.HeightMap), node, "HeightData");
                ProjectWindowUtil.ShowCreatedAsset(world);
            } 
        }
        #endif
    }
}