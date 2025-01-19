using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    public class ContextualMenuBuilder
    {
        private IGraph tree;
        private TerrainGeneratorView view;
        public ContextualMenuBuilder(IGraph tree, TerrainGeneratorView view){
            this.view = view;
            this.tree = tree;
        }
        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            GroupView actualGroup = view.Query<GroupView>().Where((GroupView a) => 
                a.ContainsPoint(a.WorldToLocal(evt.mousePosition))).First();
            
            Vector2 mousePosition = view.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            evt.menu.AppendAction("Create Node", a => ShowSearchMenu(actualGroup, a.eventInfo.localMousePosition));
            evt.menu.AppendAction("Create StickyNote", a => CreateStickyNode(actualGroup, mousePosition));
            if(actualGroup == null){
                evt.menu.AppendAction("Create Group", a => CreateGroup(mousePosition));
            }
            evt.menu.AppendSeparator();

        }
        
        private void ShowSearchMenu(GroupView groupView, Vector2 position){
            Adapter adapter = new Adapter("Create Node");
            SearcherWindow.Show(EditorWindow.focusedWindow, GetSearchItems(groupView,position), adapter, CreateNode, position, default);
        }
        
        private List<SearcherItem> GetSearchItems(GroupView ogGroup, Vector2 position){
            List<SearcherItem> items = new List<SearcherItem>();

            var types = TypeCache.GetTypesDerivedFrom<InfiniteLandsNode>();
            var sortedTypes = types.Where(a => EditorTools.GetAttribute<CustomNodeAttribute>(a) != null).OrderBy(b => {
                CustomNodeAttribute attributeA = EditorTools.GetAttribute<CustomNodeAttribute>(b);
                return attributeA.name;
            });

            foreach (var type in sortedTypes)
            {
                CustomNodeAttribute attribute = EditorTools.GetAttribute<CustomNodeAttribute>(type);
                bool validNode = attribute != null && attribute.canCreate && attribute.IsValidInTree(tree.GetType());
                int existingNodes = tree.GetNodes().Count(a => a.GetType().Equals(type));
                var status = existingNodes > 0 && attribute.singleInstance
                    ? DropdownMenuAction.Status.Disabled
                    : DropdownMenuAction.Status.Normal;

                if (validNode){
                    SearcherItem nodeItem = new SearcherItem(attribute.type+"/"+attribute.name)
                    {
                        UserData = new ItemInfo(){
                            type = type,
                            groupView = ogGroup,
                            mousePosition = position,
                        },
                    };
                    items.Add(nodeItem);
                }
            }
            return SearcherTreeUtility.CreateFromFlatList(items);
        }
        
        private bool CreateNode(SearcherItem item){
            if(item == null)
                return false;
            ItemInfo data = (ItemInfo)item.UserData;
            InfiniteLandsNode nd = tree.CreateNode(data.type,
                view.viewTransform.matrix.inverse.MultiplyPoint(data.mousePosition));
            
            NodeView nodeView = GraphViewersFactory.CreateNodeView(nd);
            view.AddNode(nodeView);
            if(data.groupView != null)
                data.groupView.AddElement(nodeView);
            return true;
        }

        private void CreateStickyNode(GroupView group, Vector2 position){
        
            StickyNoteBlock note = tree.CreateStickyNote(position);
            StickyNoteView noteView  = GraphViewersFactory.CreateStickyNoteView(note);
            view.AddStickyNote(noteView);
            if(group != null){
                group.AddElement(noteView);
            }
        }

        private void CreateGroup(Vector2 position){
            List<GraphElement> elements = view.selection.OfType<GraphElement>().Where(a => a.IsGroupable()).ToList();
            List<string> elementsGuids = elements.Select(a => a.viewDataKey).ToList();

            GroupBlock block = tree.CreateGroup("Name", position, elementsGuids);
            GroupView blockView = GraphViewersFactory.CreateGroupView(block, elements);
            view.AddGroup(blockView);
        }
    }
}