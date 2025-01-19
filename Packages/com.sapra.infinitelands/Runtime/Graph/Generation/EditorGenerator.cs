using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class EditorGenerator 
    {
        IEnumerable<InfiniteLandsNode> nodes;
        HeightOutputNode output;

        public EditorGenerator(IGraph generator){
            nodes = generator.GetNodes();
            output = generator.GetOutputNode();
        }
        public void GenerateEditorVisuals(string editorGuid, int EditorResolution, float MeshScale, int Seed, Vector2 WorldOffset, IBurstTexturePool pool)
        {
            IHavePreview[] imageMakers = nodes.OfType<IHavePreview>().Cast<IHavePreview>().Where(a => a.ShowPreview()).ToArray();
            if(imageMakers.Length <= 0)
                return;

            var manager = new DataManager();
            var indexManager = new IndexManager();
            string guid = GenerationSettings.GetGUID("E-"+editorGuid, EditorResolution, MeshScale);
            
            int layers = 0;
            int MaxResolution = EditorResolution;
            float ratio = EditorResolution/MeshScale;
            for (int i = 0; i < imageMakers.Length; i++)
            {
                if(imageMakers[i] is HeightDataGenerator heightGiver){
                    heightGiver.ValidationCheck();
                    MaxResolution = Math.Max(MaxResolution, heightGiver.PrepareNode(indexManager,ref layers, EditorResolution, ratio, guid));
                }
            }
            float FinalMeshScale = MaxResolution/ratio;

            Vector3 WorldPosition = new Vector3(WorldOffset.x, 0, WorldOffset.y);
            GenerationSettings settings = GenerationSettings.NewSettings(layers, MaxResolution, FinalMeshScale, Seed, WorldPosition, ratio, manager,indexManager,output, guid);
            settings.dependancy.Complete();

            foreach (IHavePreview node in imageMakers)
            {
                node.GenerateTexture(settings, pool);
            }
            
            manager.Return(settings.terrain.ID);
            manager.Dispose(settings.dependancy);
        }
    }
}