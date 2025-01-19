using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    [CustomNodeView(typeof(LocalInputNode))]
    public class LocalInputNodeEditor : NodeView
    {
        public LocalInputNodeEditor(InfiniteLandsNode node) : base(node)
        {
            VisualElement collapsible = this.Q<VisualElement>("collapsible-area");
            collapsible.style.display = DisplayStyle.None;
            collapsible.style.height = 0;
            titleButtonContainer.style.display = DisplayStyle.None;
            expanded = true;
            node.expanded = true;
        }

        protected override void CreateOutputs()
        {
            ports = new List<Port>();
            LocalInputNode nd = node as LocalInputNode;
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                typeof(IGive<HeightData>));

            Button btn = new Button() { text = "- select -" };
            if (nd.output != null)
            {
                btn.text = nd.output.OutputName;
                btn.bindingPath = nameof(nd.output.OutputName);
                SerializedObject bj = new SerializedObject(nd.output);
                btn.Bind(bj);
            }

            btn.clicked += () => CreateMenu(btn);
            btn.style.minWidth = 80;
            btn.style.marginTop = 0;
            btn.style.marginBottom = 0;

            var label = output.contentContainer.Q<Label>("type");
            label.style.color = new StyleColor(new Color(0, 0, 0, 0));
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            label.Add(btn);


            if (output != null)
            {
                output.portName = "localInput";
                outputContainer.Add(output);
                ports.Add(output);
            }
        }
        protected override void CreateInputs()
        {}

        void CreateMenu(Button btn)
        {
            GenericMenu menu = new GenericMenu();
            string parentAsset = AssetDatabase.GetAssetPath(this.node);
            object[] asset = AssetDatabase.LoadAllAssetsAtPath(parentAsset);
            LocalInputNode nd = node as LocalInputNode;
            foreach (object st in asset)
            {
                if (st is LocalOutputNode newNode)
                {
                    menu.AddItem(new GUIContent(newNode.OutputName), newNode.Equals(nd.output), () =>
                    {
                        nd.output = newNode;
                        Connection connection = new Connection(){
                            provider = newNode,
                            providerPortName = newNode.name,
                            inputPortName = nameof(nd.output)
                        };
                        nd.AddConnection(connection);
                        btn.bindingPath = nameof(newNode.OutputName);
                        SerializedObject bj = new SerializedObject(newNode);
                        btn.Bind(bj);
                        nd.OnValuesUpdated?.Invoke();
                    });
                }
            }

            menu.ShowAsContext();
        }
    }
}