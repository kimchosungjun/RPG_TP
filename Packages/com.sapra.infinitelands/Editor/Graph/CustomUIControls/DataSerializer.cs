using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using System.Linq;
using System;

using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
namespace sapra.InfiniteLands.Editor{
    public class DataSerializer 
    {       
        #region Legacy Contextual Menu
        public static void UnserializeAndPaste(Vector2 position, string data, IGraph tree)
        {
            SerializedGuids serializedG = JsonUtility.FromJson<SerializedGuids>(data);

            Dictionary<string, InfiniteLandsNode> NewNodesGenerated = new Dictionary<string, InfiniteLandsNode>();
            Dictionary<string, StickyNoteBlock> NewNotes = new Dictionary<string, StickyNoteBlock>();
            List<GroupBlock> NewGroups = new List<GroupBlock>();

            //Add the items
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();

            foreach(SerializedItem serializedItem in serializedG.guids){
                Type targetType = serializedItem.TheType();
                Vector2 desiredPosition = serializedItem.position-serializedG.center+position;
                string OriginalGuid = serializedItem.guid;
                if(typeof(InfiniteLandsNode).IsAssignableFrom(targetType)){
                    CustomNodeAttribute attribute = EditorTools.GetAttribute<CustomNodeAttribute>(targetType);
                    bool validNode = attribute != null && attribute.canCreate && attribute.IsValidInTree(tree.GetType());

                    if(validNode){
                        InfiniteLandsNode node = tree.CreateNodeFromJson(targetType, serializedItem.item, desiredPosition);
                        NewNodesGenerated.Add(OriginalGuid, node);
                    }
                    continue;
                }
                
                if(typeof(GroupBlock).IsAssignableFrom(targetType)){
                    GroupBlock block = tree.CreateGroupFromJson(serializedItem.item, desiredPosition);
                    NewGroups.Add(block);
                    continue;
                }

                if(typeof(StickyNoteBlock).IsAssignableFrom(targetType)){
                    StickyNoteBlock noteView = tree.CreateStickyNoteFromJson(serializedItem.item, desiredPosition);
                    NewNotes.Add(OriginalGuid, noteView);
                    continue;
                }
            }

            //Connect Nodes
            foreach(KeyValuePair<string, InfiniteLandsNode> nd in NewNodesGenerated){
                List<Connection> originalConnections = new List<Connection>(nd.Value.GetConnections());
                nd.Value.ClearConnections();

                foreach(Connection connection in originalConnections){
                    if(NewNodesGenerated.TryGetValue(connection.provider.guid, out InfiniteLandsNode possibleConnection)){
                        tree.Connect(nd.Value, connection.inputPortName, possibleConnection, connection.providerPortName);
                    }
                    else{
                        tree.Connect(nd.Value, connection.inputPortName, connection.provider, connection.providerPortName);
                    }
                }
            }

            //Create Groups
            foreach(GroupBlock group in NewGroups){
                List<string> newGuids = new List<string>();
                foreach(string guid in group.elementGuids){
                    if(NewNodesGenerated.TryGetValue(guid, out InfiniteLandsNode newNode)){
                        newGuids.Add(newNode.guid);
                    }else if(NewNotes.TryGetValue(guid, out StickyNoteBlock newNote)){
                        newGuids.Add(newNote.guid);
                    }
                }
                group.elementGuids.Clear();
                tree.AddElementsToGroup(group, newGuids);
            }

            Undo.SetCurrentGroupName(string.Format("{0}: Pasting data", tree.GetType().Name));
            Undo.CollapseUndoOperations(curGroupID);
        }

        public static bool CanPaste(string data)
        {
            try{
                SerializedGuids serializedG = JsonUtility.FromJson<SerializedGuids>(data);
                if(serializedG.guids.Count <= 0)
                    return false;
                foreach(SerializedItem guid in serializedG.guids){
                    if(!guid.ValidSerialization())
                        return false;
                }
                return true;
            }
            catch(Exception){
                return false;
            }
        }

        public static bool CanPasteInto(string data, Type type)
        {
            try{
                SerializedGuids serializedG = JsonUtility.FromJson<SerializedGuids>(data);
                if(serializedG.guids.Count != 1)
                    return false;
                foreach(SerializedItem guid in serializedG.guids){
                    if(!guid.ValidSerialization())
                        return false;                    
                }
                return true;
            }
            catch(Exception){
                return false;
            }
        }

        private static string ClearData(string data){
            if (data.StartsWith("application/vnd.unity.graphview.elements"))
                return data.Substring("application/vnd.unity.graphview.elements".Length + 1);
            else
                return data;
        }
        private static string GetClipboard(){
            return ClearData(EditorGUIUtility.systemCopyBuffer);
        }

        public static void BuildContextualMenu(ContextualMenuPopulateEvent evt){
            if(evt.target is NodeView targetNode){
                string clipboard = GetClipboard();
                bool isEnabled = CanPasteInto(clipboard, targetNode.node.GetType());
                evt.menu.AppendAction("Paste Values", a => PasteComponentValues(targetNode.node, clipboard), 
                    isEnabled ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();
                
            }
        }
        private static void PasteComponentValues(InfiniteLandsNode target, string data){
            SerializedGuids serializedG = JsonUtility.FromJson<SerializedGuids>(data);
            SerializedItem serializedItem = serializedG.guids[0];

            Type originalType = serializedItem.TheType();
            InfiniteLandsNode result = ScriptableObject.CreateInstance(originalType) as InfiniteLandsNode;
            JsonUtility.FromJsonOverwrite(serializedItem.item, result);
            CopyDataFromTo(result, target);
            GameObject.DestroyImmediate(result);
        }

        private static void CopyDataFromTo(InfiniteLandsNode from, InfiniteLandsNode to){
            FieldInfo[] FromFields = from.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(a => 
                    a.GetCustomAttribute<HideInInspector>() == null &&
                    a.GetCustomAttribute<InputAttribute>() == null &&
                    !a.FieldType.IsClass).ToArray();
            FieldInfo[] TargetFields = to.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(a => 
                    a.GetCustomAttribute<HideInInspector>() == null &&
                    a.GetCustomAttribute<InputAttribute>() == null &&
                    !a.FieldType.IsClass).ToArray();
            bool swaped = false;
            foreach(FieldInfo field in FromFields){
                FieldInfo similarField = TargetFields.FirstOrDefault(a => EqualFieldInfo(field, a));
                if(similarField != null){
                    similarField.SetValue(to, field.GetValue(from));
                    swaped = true;
                }
            }
            if(swaped)
                to.OnValuesUpdated?.Invoke();
        }

        private static bool EqualFieldInfo(FieldInfo A, FieldInfo B){
            return A.FieldType.Equals(B.FieldType) &&
                    A.Name.Equals(B.Name);
        }
        public static string SerializeGraphEelements(IEnumerable<GraphElement> elements)
        {
            IEnumerable<IRenderSerializableGraph> ValidData = elements.OfType<IRenderSerializableGraph>();
            SerializedGuids guids = new SerializedGuids();
            Vector2 center = Vector2.zero;
            int validCount = 0;
            foreach(IRenderSerializableGraph element in ValidData){
                object serializedData = element.GetDataToSerialize();
                CustomNodeAttribute attribute = EditorTools.GetAttribute<CustomNodeAttribute>(serializedData.GetType());
                bool validNode = attribute == null || (attribute != null && attribute.canCreate);
                if(validNode){
                    validCount++;
                    SerializedItem sn = new SerializedItem(element);
                    guids.guids.Add(sn);
                    center += element.GetPosition();
                }
            }
            if(validCount > 0)
                guids.center = center/validCount;

            string data = JsonUtility.ToJson(guids);
            return data;
        }

        [Serializable]
        public class SerializedGuids{
            public Vector2 center = Vector2.zero;
            public List<SerializedItem> guids = new List<SerializedItem>();
        }

        [Serializable]
        public class SerializedItem{
            public string type;
            public string guid;
            public Vector2 position;
            public string item;
            public SerializedItem(IRenderSerializableGraph serializable){
                object data = serializable.GetDataToSerialize();
                this.type = data.GetType().AssemblyQualifiedName;
                this.guid = serializable.GetGUID();
                this.position = serializable.GetPosition();
                this.item = JsonUtility.ToJson(data);
            }
            public Type TheType() => Type.GetType(type);

            public bool ValidSerialization(){
                return item != "" && TheType() != null;
            }
        }
        #endregion
    }
}