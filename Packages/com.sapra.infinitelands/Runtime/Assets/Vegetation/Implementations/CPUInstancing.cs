using System.Collections.Generic;
using UnityEngine;
using sapra.InfiniteLands.NaughtyAttributes;
using System.Linq;
using static sapra.InfiniteLands.IHoldVegetation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sapra.InfiniteLands{  
    [AssetNodeAttribute(typeof(VegetationOutputNode))]
    [CreateAssetMenu(fileName = "Vegetation Asset", menuName = "InfiniteLands/Assets/Vegetation/CPUInstancing")]
    public class CPUInstancing : VegetationAsset, IHoldVegetation
    {
        [SerializeField] public bool _skipRendering;

        [Header("Configuration")] [Min(0.0001f)]
        public float DistanceBetweenItems = 100;
        [Min(0.01f)] public float MinimumScale = 1;
        [Min(0.01f)] public float MaximumScale = 1;
        public float VerticalPosition = 0;
        public AlignmentMode AlignToGround = AlignmentMode.Up;
        public DensityHeightMode DensityAffectsHeight = DensityHeightMode.Independent;
        [Range(0,1)]public float PositionRandomness = 1;
        [Min(0)]public float TextureRandomnessDistance;
        
        [Header("Main Data")] 
        [AllowNesting] public GameObject InstanceObject;
        [AllowNesting] public Material[] Materials;

        [Header("Rendering")] 
        public float ViewDistance = 200;
       
        [SerializeField] [Header("Debugging")] private bool DrawDistances;
        public bool DrawBoundigBoxes;

        public float GetViewDistance() => ViewDistance;
        public float GetDistanceBetweenItems() => DistanceBetweenItems;

        public bool SkipRendering()=>_skipRendering;
        public Vector2 GetMinimumMaximumScale() => new Vector2(MinimumScale, MaximumScale);
        public float GetVerticalPosition() =>VerticalPosition;
        public AlignmentMode GetAlignmentMode() => AlignToGround;
        public DensityHeightMode GetDensityMode() => DensityAffectsHeight;
        public float GetTextureRandomness() => TextureRandomnessDistance;
        public float ExtraVerticalBound() => 0;
        public float GetPositionRandomness() => PositionRandomness;

        public List<GraphicsBuffer.IndirectDrawIndexedArgs> InitializeMaterialsAndMeshes(IPaintTerrain painter){
            if(Materials.Length > 0){
                            
                var distinctMats = Materials.Distinct().ToArray();
                foreach(Material mat in distinctMats){
                    painter.AssignMaterials(mat);
                    mat.enableInstancing = true;
                }
            }
            return new List<GraphicsBuffer.IndirectDrawIndexedArgs>(){
                new GraphicsBuffer.IndirectDrawIndexedArgs
                {
                    indexCountPerInstance = 0,
                    instanceCount = 0,
                    startIndex = 0,
                    baseVertexIndex = 0,
                    startInstance = 0
                }
            };
        }

        public void GizmosDrawDistances(Vector3 position){
            if(DrawDistances){
                #if UNITY_EDITOR
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(position, Vector3.up, ViewDistance);
                #endif
            }
        }

        public IRenderVegetation GetRenderer(IPaintTerrain painter, IGenerate<TextureResult> TextureMaker,PointStore store, VegetationSettings settings, List<Camera> cameras, 
            RenderTexture[] depthtextures, Transform colliderParent) =>  new Vegetation_CPUInstancer(this, painter,TextureMaker, store, settings, cameras, depthtextures, colliderParent);
    
        public ICompact GetCompactor(VegetationSettings settings, int TotalChunksCount, List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments)
        {
            return new CountCompactDraw(this, settings, TotalChunksCount, arguments);
        }

        public override object Preview( )=> null;
    }
}