using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    [CustomNodeView(typeof(LocalOutputNode))]
    public class LocalOutputNodeEditor : NodeView
    {
        public LocalOutputNodeEditor(InfiniteLandsNode node) : base(node)
        {
            VisualElement collapsible = this.Q<VisualElement>("collapsible-area");
            collapsible.style.display = DisplayStyle.None;
            collapsible.style.height = 0;

            titleButtonContainer.style.display = DisplayStyle.None;
            expanded = true;
            node.expanded = true;
        }

        protected override void CreateInputs()
        {
            ports = new List<Port>();
                
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(IGive<HeightData>));

            LocalOutputNode nd = node as LocalOutputNode;
            TextField field = new TextField
            {
                name = nd.OutputName,
                value = nd.OutputName,
                bindingPath = nameof(nd.OutputName)
            };
            field.Bind(new SerializedObject(node));
            field.style.minWidth = 80;

            var label = output.contentContainer.Q<Label>("type");
            label.style.color = new StyleColor(new Color(0, 0, 0, 0));
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            label.Add(field);


            if (output != null)
            {
                output.portName = nameof(nd.inputMap);
                inputContainer.Add(output);
                ports.Add(output);
            }
        }
    }
}