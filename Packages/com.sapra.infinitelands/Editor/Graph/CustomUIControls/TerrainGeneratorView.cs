using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System;

namespace sapra.InfiniteLands.Editor{
    #if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    #endif

    public partial class TerrainGeneratorView : GraphView
    {
        #if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<TerrainGeneratorView, UxmlTraits>{}
        #endif

        private ContextualMenuBuilder contextualMenu;
        private TerrainGenerator tree;

        public Action RegenerateNodeSystem;
        private Vector2 MousePosition;
        public TerrainGeneratorView()
        {
            Insert(0, new GridBackground());
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Packages/com.sapra.infinitelands/Editor/UIBuilder/InfiniteLandsGraphEditor.uss");
            if(styleSheets != null && styleSheet != null)
                styleSheets.Add(styleSheet);
            else
                return;

            UnregisterCallback<MouseMoveEvent>(MouseMove);
            canPasteSerializedData -= DataSerializer.CanPaste;
            serializeGraphElements -= DataSerializer.SerializeGraphEelements; 
            unserializeAndPaste -= UnserializeAndPaste;
            deleteSelection -= DeleteSelection;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale*1.5f);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new DragAndDropManipulator(this));
            
            RegisterCallback<MouseMoveEvent>(MouseMove);
            serializeGraphElements += DataSerializer.SerializeGraphEelements; 
            canPasteSerializedData += DataSerializer.CanPaste;
            unserializeAndPaste += UnserializeAndPaste;
            deleteSelection += DeleteSelection;
        }

        private void ReloadView(){           
            graphViewChanged -= OnGraphViewChanged;
            elementsAddedToGroup -= OnElementsAddedToGroup;
            elementsRemovedFromGroup -= OnElementsRemovedToGroup;
            viewTransformChanged -= UpdateGraphViewPosition;

            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;
            elementsAddedToGroup += OnElementsAddedToGroup;
            elementsRemovedFromGroup += OnElementsRemovedToGroup;
            viewTransformChanged += UpdateGraphViewPosition;

            LoadTreeView();
            RegenerateNodeSystem?.Invoke();
        }

        public void Initialize(TerrainGenerator terrainGenerator, InfiniteLandsGraphEditor editor){            
            this.tree = terrainGenerator;
            contextualMenu = new ContextualMenuBuilder(terrainGenerator, this);
            ReloadView();
        }
        
        private void UpdateGraphViewPosition(GraphView graphView){
            tree.position = graphView.viewTransform.position;
            tree.scale = graphView.viewTransform.scale;
        }
        
        private void MouseMove(MouseMoveEvent moveEvent){
            MousePosition = moveEvent.mousePosition;
        }
    
        public NodeView FindNodeView(InfiniteLandsNode node)
        {
            Node nd = GetNodeByGuid(node.guid);
            if (nd is NodeView view)
            {
                return view;
            }
            else
                Debug.Log("Invalid node view with " + node.ToString());

            return null;
        }

        private void LoadTreeView(){
            if(tree.scale.Equals(Vector2.zero))
                tree.scale = Vector2.one;

            UpdateViewTransform(tree.position, tree.scale);
            tree.nodes.RemoveAll(a => a == null);
            tree.stickyNotes.RemoveAll(a => a == null);
            tree.groups.RemoveAll(a => a == null);

            tree.nodes.ForEach(n => {
                NodeView nodeView = GraphViewersFactory.CreateNodeView(n);
                AddNode(nodeView);
            });
            
            foreach(var n in tree.nodes){
                List<Connection> connections = n.GetConnections();
                foreach(Connection c in connections)
                {
                    NodeView reciverView = FindNodeView(n);
                    NodeView providerView = FindNodeView(c.provider);
                    if (providerView != null && reciverView != null)
                    {
                        Port inputPort = providerView.ports.Find(a => a.portName.Equals(c.providerPortName));
                        Port outputPort = reciverView.ports.Find(a => a.portName.Equals(c.inputPortName));

                        if (inputPort != null && outputPort != null)
                        {
                            Edge edge = inputPort.ConnectTo(outputPort);
                            AddElement(edge);
                        }
                    }
                };
            }

            tree.nodes.ForEach(n => {
                NodeView nv = FindNodeView(n);
                nv.expanded = n.expanded;
            });

            tree.stickyNotes.ForEach(sn => AddElement(GraphViewersFactory.CreateStickyNoteView(sn)));

            tree.groups.ForEach(g => {
                List<GraphElement> elements = g.elementGuids.Select(a => GetElementByGuid(a)).ToList();
                GroupView groupView = GraphViewersFactory.CreateGroupView(g, elements);
                AddGroup(groupView);
            });
        }

        private void OnElementsAddedToGroup(Group group, IEnumerable<GraphElement> elements)
        {
            GroupView groupView = group as GroupView;
            List<string> guids = elements.Select(a => a.viewDataKey).ToList();
            tree.AddElementsToGroup(groupView.group, guids);
        }

        private void OnElementsRemovedToGroup(Group group, IEnumerable<GraphElement> elements)
        {
            GroupView groupView = group as GroupView;
            List<string> guids = elements.Select(a => a.viewDataKey).ToList();
            tree.RemoveElementsToGroup(groupView.group, guids);
        }

        public void OnAssetDropped(IAsset asset, Vector2 position){           
            var nodeType = EditorTools.GetAttribute<AssetNodeAttribute>(asset.GetType());
            if(nodeType != null){
                InfiniteLandsNode nd = tree.CreateNode(nodeType.DefaultNode, viewTransform.matrix.inverse.MultiplyPoint(position));
                ILoadAsset loader = nd as ILoadAsset;
                if(loader != null){
                    loader.SetAsset(asset);
                }
                NodeView nodeView = GraphViewersFactory.CreateNodeView(nd);
                AddNode(nodeView);
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if(graphViewChange.elementsToRemove != null){
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView provider = edge.output.node as NodeView;
                        NodeView reciver = edge.input.node as NodeView;
                        tree.RemoveConnection(reciver.node, edge.input.portName, provider.node, edge.output.portName);
                    }
                });
            }

            if(graphViewChange.edgesToCreate != null){
                for(int i = graphViewChange.edgesToCreate.Count-1; i >= 0; i--){
                    Edge edge = graphViewChange.edgesToCreate[i];
                    NodeView provider = edge.output.node as NodeView;
                    NodeView reciver = edge.input.node as NodeView;
                    if(!tree.Connect(reciver.node, edge.input.portName, provider.node, edge.output.portName))
                        graphViewChange.edgesToCreate.RemoveAt(i);
                }
                RegenerateNodeSystem?.Invoke();
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endport =>
                endport != startPort &&
                endport.direction != startPort.direction &&
                endport.node != startPort.node &&
                endport.portType.Equals(startPort.portType)).ToList();
        }

        private void CollapseExpandNodes(bool state){
            if(!state)
                HideShowPreviews(state);
            tree.nodes.ForEach(n => {
                n.expanded = state;
                NodeView node = FindNodeView(n);
                node.expanded = state;
            });
        }

        private void HideShowPreviews(bool state){
            if(state)
                CollapseExpandNodes(state);
            tree.nodes.ForEach(n => {
                if(n is IHavePreview img){
                    img.TogglePreview(state);
                }
            });            
            RegenerateNodeSystem?.Invoke();
        }

        public void AddStickyNote(StickyNoteView noteView){
            AddElement(noteView);
        }

        public void AddGroup(GroupView group){
            group.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            AddElement(group);
        }

        public void AddNode(NodeView node){
            node.node.OnValuesUpdated += RegenerateNodeSystem;
            AddElement(node);
        }


        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {   
            contextualMenu.BuildContextualMenu(evt);
            DataSerializer.BuildContextualMenu(evt);

            evt.target = this;
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("View/Expand Nodes", a => CollapseExpandNodes(true));
            evt.menu.AppendAction("View/Collapse Nodes", a => CollapseExpandNodes(false));
            evt.menu.AppendSeparator("View/");
            evt.menu.AppendAction("View/Expand Preview", a => HideShowPreviews(true));
            evt.menu.AppendAction("View/Collapse Preview", a => HideShowPreviews(false));
            evt.menu.AppendSeparator("View/");
            evt.menu.AppendAction("View/Fit", a => FrameAll());

            evt.StopImmediatePropagation();
        }

        #region Legacy Contextual Menu
        private void DeleteSelection(string operationName, AskUser askUser){
            List<GraphElement> elements = selection.OfType<GraphElement>().ToList();

/*             IEnumerable<NodeView> nodesToDelete = selection.OfType<NodeView>();
            IEnumerable<StickyNoteView> stickyNotesToDelete = selection.OfType<StickyNoteView>();
            IEnumerable<GroupView> groupsToDelete = selection.OfType<GroupView>();
            IEnumerable<Edge> edgesToDelete = selection.OfType<Edge>();

            IEnumerable<InfiniteLandsNode> nodes = nodesToDelete.Select(a => a.node);
            IEnumerable<StickyNoteBlock> notes = stickyNotesToDelete.Select(a => a.note);
            IEnumerable<GroupBlock> groups = groupsToDelete.Select(a => a.group);
            IEnumerable<Edge> edgesToDelete = selection.OfType<Edge>(); */
            Undo.IncrementCurrentGroup();
            var curGroupID = Undo.GetCurrentGroup();
            
            foreach(GraphElement element in elements){
                if(element is NodeView nodeView){
                    CustomNodeAttribute attribute = EditorTools.GetAttribute<CustomNodeAttribute>(nodeView.node.GetType());
                    if (attribute != null && !attribute.canCreate)
                        continue;
                    tree.Remove(nodeView.node);
                    RemoveElement(nodeView);
                }
                if(element is GroupView groupView){
                    tree.Remove(groupView.group);
                    RemoveElement(groupView);
                }
                if(element is StickyNoteView stickyNote){
                    tree.Remove(stickyNote.note);
                    RemoveElement(stickyNote);
                }
                if(element is Edge edge){
                    NodeView provider = edge.output.node as NodeView;
                    NodeView reciver = edge.input.node as NodeView;
                    tree.RemoveConnection(reciver.node, edge.input.portName, provider.node, edge.output.portName);
                    RemoveElement(edge);
                }
            }

            foreach(InfiniteLandsNode nd in tree.nodes){
                nd.ClearInvalidConnections();
            }
            Undo.SetCurrentGroupName(string.Format("{0}: Deleting data", tree.GetType().Name));
            Undo.CollapseUndoOperations(curGroupID);
            
            RegenerateNodeSystem?.Invoke();
            ReloadView();
        }

        private void UnserializeAndPaste(string operationName, string data)
        {
            Vector2 position = viewTransform.matrix.inverse.MultiplyPoint(MousePosition);
            DataSerializer.UnserializeAndPaste(position, data, tree);
            RegenerateNodeSystem?.Invoke();            
            ReloadView();
        }
        #endregion

    }
}