using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace sapra.InfiniteLands.Editor{
    public static class GraphViewersFactory
    {
        #region Nodes
        public static NodeView CreateNodeView(InfiniteLandsNode node)
        {
            Type nodeViewType = GetNodeViewType(node.GetType());

            if (nodeViewType == null)
                nodeViewType = typeof(NodeView);
            NodeView nodeView = Activator.CreateInstance(nodeViewType, new object[] { node }) as NodeView;
            return nodeView;
        }

        #endregion

        #region StickyNotes
        public static StickyNoteView CreateStickyNoteView(StickyNoteBlock sn){
            StickyNoteView note = new StickyNoteView(sn)
            {
                title = sn.title,
                contents = sn.content,
                viewDataKey = sn.guid
            };
            return note;
        }
        #endregion

        #region Groups
        public static GroupView CreateGroupView(GroupBlock g, List<GraphElement> selection)
        {
            GroupView group = new GroupView(g)
            {
                title = g.Name,
                viewDataKey = g.guid,
            };

            selection.ForEach(n =>
            {
                if(n!= null)
                    group.AddElement(n);
            });

            return group;
        }
        #endregion

        private static Type GetNodeViewType(Type type)
        {
            GetNodeViewAndAttributeType(type, out Type nodeType, out _);
            return nodeType;
        }

        private static void GetNodeViewAndAttributeType(Type type, out Type nodeType,
            out CustomNodeViewAttribute attributeInNode)
        {
            var types = TypeCache.GetTypesDerivedFrom(typeof(NodeView)).ToList();
            foreach (Type foundTypes in types)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(foundTypes);
                foreach (Attribute attribute in attrs)
                {
                    if (attribute is CustomNodeViewAttribute customNode)
                    {
                        if (customNode.target.IsEquivalentTo(type))
                        {
                            attributeInNode = customNode;
                            nodeType = foundTypes;
                            return;
                        }
                    }
                }
            }

            attributeInNode = null;
            nodeType = null;
            return;
        }

    }
}