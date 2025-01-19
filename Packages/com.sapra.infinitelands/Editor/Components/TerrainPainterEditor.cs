using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace sapra.InfiniteLands.Editor{
    [CustomEditor(typeof(TerrainPainter))]
    [CanEditMultipleObjects]
    public class TerrainPainterEditor : UnityEditor.Editor
    {
        private GUIStyle highlightButton;
        private GUIStyle defaultButton;

        public override void OnInspectorGUI()
        {
            if(highlightButton == null){
                highlightButton = new GUIStyle(GUI.skin.GetStyle("Button"));
                highlightButton.fontStyle = FontStyle.Bold;
            }

            if(defaultButton == null){
                defaultButton = new GUIStyle(GUI.skin.GetStyle("Button"));
            }
            base.OnInspectorGUI();
            var painter = target as TerrainPainter;
            IEnumerable<AssetWithType> allAssets = painter.assets;
            if(allAssets == null)
                return;
                
            if(painter.ProceduralTexturing)
                GUI.color = Color.white;
            else
                GUI.color = Color.gray;
            if(GUILayout.Button("Procedural", painter.ProceduralTexturing ?highlightButton : defaultButton)){
                painter.ChangeToTexture(default, true);
            }
            GUI.color = Color.gray;

            if(!painter.ProceduralTexturing && painter.GetDesired == null)
                GUI.color = Color.white;
            if(GUILayout.Button("Normal Map", !painter.ProceduralTexturing && painter.GetDesired == null ? highlightButton : defaultButton)){
                painter.ChangeToTexture(default, false);
            }
            GUI.color = Color.gray;

            foreach(AssetWithType asset in allAssets){
                GUI.color = Color.white;
                EditorGUILayout.LabelField(asset.originalType.Name);
                GUI.color = Color.gray;

                foreach(IAsset value in asset.assets){
                    if(painter.GetDesired != null && painter.GetDesired.Equals(value))
                        GUI.color = Color.white;
                    if(GUILayout.Button(value.name, painter.GetDesired != null && painter.GetDesired.Equals(value) ? highlightButton : defaultButton)){
                        painter.ChangeToTexture(value, false);
                    }
                    GUI.color = Color.gray;

                }
            }
        }
    }
}