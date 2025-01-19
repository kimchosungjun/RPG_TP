using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace sapra.InfiniteLands.Editor
{
    [CustomEditor(typeof(TerrainGenerator), true)]
    [CanEditMultipleObjects]
    internal class TerrainGeneratorEditor : UnityEditor.Editor
    {
        private void OnSceneDrag(SceneView sceneView, int index){                    
            Event e = Event.current;
            GameObject go = HandleUtility.PickGameObject(e.mousePosition, false);

            if (e.type == EventType.DragUpdated) {
                if (go) {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                } else {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                e.Use();
            } else if (e.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                e.Use();
            
                SingleChunkVisualizer dhcomponent = go?.GetComponent<SingleChunkVisualizer>();
                TerrainGenerator terrainGenerator = target as TerrainGenerator;
                if (!dhcomponent) {
                    go = new GameObject(target.name);
                    Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Plane ground = new Plane(Vector3.up, Vector3.zero);
                    ground.Raycast(r, out float distance);
                    Vector3 pos = r.GetPoint(distance);
                    go.transform.position = pos;
                    dhcomponent = go.AddComponent<SingleChunkVisualizer>();
                    go.AddComponent<PointStore>();
                    go.AddComponent<MeshMaker>();
                    TerrainPainter painter = go.AddComponent<TerrainPainter>();
                    Shader sdr = Resources.Load<Shader>("Shaders/Terrain/Terrain");
                    painter.terrainMaterial = new(sdr);

                    go.AddComponent<VegetationRenderer>();
                    Selection.activeGameObject = go;
                }
                dhcomponent.ChangeGenerator(terrainGenerator);
                dhcomponent.ForceGeneration(false);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            } 

        }
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor")){
                InfiniteLandsGraphEditor.OpenGraphAsset(target as TerrainGenerator);
            }

            EditorGUI.BeginDisabledGroup(true);
            serializedObject.Update();
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
        }
    }
}