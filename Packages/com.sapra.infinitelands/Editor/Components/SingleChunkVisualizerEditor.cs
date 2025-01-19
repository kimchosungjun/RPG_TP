using UnityEngine;
using UnityEditor;

namespace sapra.InfiniteLands.Editor
{
    [CustomEditor(typeof(SingleChunkVisualizer))]
    [CanEditMultipleObjects]
    internal class SingleChunkVisualizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Sync & Regenerate")){
                SingleChunkVisualizer generator = target as SingleChunkVisualizer;
                if (generator) 
                    generator.ForceGeneration(true);
            }
        }
    }
}