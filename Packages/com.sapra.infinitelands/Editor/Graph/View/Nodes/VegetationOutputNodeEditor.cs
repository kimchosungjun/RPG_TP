using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    [CustomNodeView(typeof(VegetationOutputNode))]
    public class VegetationOutputNodeEditor : NodeView
    {
        private MeshPreview preview;
   
        public VegetationOutputNodeEditor(InfiniteLandsNode node) : base(node)
        {
            RegisterCallback<DetachFromPanelEvent>(c => OnDisable());
        }

        public void OnInspectorGUI(IHavePreview generator)
        {
            UpdatePreview(generator);
            var rect = GUILayoutUtility.GetRect(200, 200);
            preview?.OnPreviewGUI(rect, "TextField");
        }

        protected override VisualElement PreviewVisualElement(IHavePreview generator){
            return new IMGUIContainer(() => { OnInspectorGUI(generator); });
        }

        protected override void UpdatePreview(IHavePreview generator){
            object targetMesh = generator.GetTemporalTexture();
            if(targetMesh == null)
                return;
            Mesh m = targetMesh as Mesh;
            if (preview == null && m != null)
                preview = new MeshPreview(m);
        }

        private void OnDisable()
        {
            if (preview != null)
            {
                preview.Dispose();
                preview = null;
            }
        }
    }
}