using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    [CustomNodeView(typeof(HeightOutputNode))]
    public class HeightOutputNodeEditor : NodeView
    {
        public HeightOutputNodeEditor(InfiniteLandsNode node) : base(node)
        {
            VisualElement collapsible = this.Q<VisualElement>("collapsible-area");
            collapsible.style.display = DisplayStyle.None;
            collapsible.style.height = 0;
            titleButtonContainer.style.display = DisplayStyle.None;
            expanded = true;
            node.expanded = true;
        }
    }
}