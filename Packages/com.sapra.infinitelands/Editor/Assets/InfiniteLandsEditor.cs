using UnityEditor;

namespace sapra.InfiniteLands.Editor{
    [CustomEditor(typeof(InfiniteLandsNode), true)]
    [CanEditMultipleObjects]
    public class InfiniteLandsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            serializedObject.Update();
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
        }
    }
}