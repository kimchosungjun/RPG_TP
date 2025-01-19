using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using sapra.InfiniteLands.NaughtyAttributes;
using static sapra.InfiniteLands.IHoldVegetation;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sapra.InfiniteLands{  
    [AssetNodeAttribute(typeof(VegetationOutputNode))]
    [CreateAssetMenu(fileName = "Vegetation Asset", menuName = "InfiniteLands/Assets/Vegetation/GPUInstancing")]
    public class GPUInstancing : VegetationAsset, IHoldVegetation
    {            
        private static readonly int
            lodDistanceID = Shader.PropertyToID("_LodDistance"),
            shadowDistanceID = Shader.PropertyToID("_ShadowDistance"),
            minBoundsID = Shader.PropertyToID("_MinBounds"),
            maxBoundsID = Shader.PropertyToID("_MaxBounds"),
            halfInstancesDistanceID = Shader.PropertyToID("_HalfInstancesDistance"),
            lodCountID = Shader.PropertyToID("_LODCount");

        [SerializeField] private bool _skipRendering;
        
        [Header("Configuration")] 
        [Min(0.01f)] [SerializeField] private float DistanceBetweenItems = 100;
        [Min(0.01f)] [SerializeField] private float MinimumScale = 1;
        [Min(0.01f)] [SerializeField] private float MaximumScale = 1;
        [SerializeField] private float VerticalPosition = 0;
        [SerializeField] private AlignmentMode AlignToGround = AlignmentMode.Up;
        [SerializeField] private DensityHeightMode DensityAffectsHeight = DensityHeightMode.Independent;
        [Range(0,1)] [SerializeField] private float PositionRandomness = 1;
        [Min(0)] [SerializeField] private float TextureRandomnessDistance;

        [Header("Main Data")] 
        [SerializeField] private bool CastShadows;
        public bool GenerateColliders;
        [AllowNesting] [ShowIf("GenerateColliders")] public Collider ColliderObject;
        public bool CrossFadeDithering = true;

        [Header("Automatic Generation")] 
        [SerializeField] private LODGroup LodGroups;

        [Header("Manual Generation")] 
        [AllowNesting][EnableIf("enableAutomatic")] [SerializeField] private MeshLOD[] LOD;
        private bool HasLods => LOD != null && LOD.Length > 1;
        [AllowNesting][ShowIf("HasLods")] [SerializeField] private float HighLodDistance = 50;
        [AllowNesting][ShowIf("HasLods")][Min(0)] [SerializeField] private int ShadowsLODOffset = 0;

        [Header("Rendering")] 
        [SerializeField] private float ViewDistance = 200;
        [AllowNesting] [ShowIf("CastShadows")] [SerializeField] private float ShadowDistance = 100;
        private float _shadowDistance => Mathf.Min(ShadowDistance, ViewDistance);
        [AllowNesting][ShowIf("GenerateColliders")] public float CollisionDistance = 30;
                
        [SerializeField] private bool HalfInstancesAtDistance;
        [AllowNesting][ShowIf("HalfInstancesAtDistance")] [SerializeField] private float HalfInstancesDistance = 30;      
       
        [SerializeField] [Header("Debugging")] private bool DrawDistances;
        public bool DrawBoundigBoxes;

        private int MaxSubMeshCount;
        private int LODLength;
        private int MaxShadowLOD;

        private Vector3 MaxMeshBounds;
        private Vector3 MinMeshBounds;
        private bool enableAutomatic => LodGroups == null;

        public bool SkipRendering() => _skipRendering;
        public float ExtraVerticalBound() => MaxMeshBounds.y;
        public Vector2 GetMinimumMaximumScale() => new Vector2(MinimumScale, MaximumScale);
        public float GetVerticalPosition() => VerticalPosition;
        public AlignmentMode GetAlignmentMode() => AlignToGround;
        public DensityHeightMode GetDensityMode() => DensityAffectsHeight;
        public float GetTextureRandomness() => TextureRandomnessDistance;
        public float GetViewDistance() => ViewDistance;
        public float GetDistanceBetweenItems() => DistanceBetweenItems;
        public float GetPositionRandomness() => PositionRandomness;
        
        protected override void OnValidate(){
            base.OnValidate();
            ShadowsLODOffset = Mathf.Clamp(ShadowsLODOffset, 0, LOD.Length-1);
        }
        public void SetVisibilityShaderData(CommandBuffer bf, ComputeShader compute){
            //Calculate Positions Data
            bf.SetComputeFloatParam(compute, lodDistanceID, HasLods? HighLodDistance:ViewDistance);
            bf.SetComputeFloatParam(compute, shadowDistanceID, _shadowDistance);
            bf.SetComputeIntParam(compute,lodCountID, LOD.Length);
            bf.SetComputeFloatParam(compute, halfInstancesDistanceID, HalfInstancesAtDistance?HalfInstancesDistance:ViewDistance);
            bf.SetComputeVectorParam(compute, minBoundsID, MinMeshBounds);
            bf.SetComputeVectorParam(compute, maxBoundsID, MaxMeshBounds);
        }
        public List<GraphicsBuffer.IndirectDrawIndexedArgs> InitializeMaterialsAndMeshes(IPaintTerrain painter){
            LODLength = Mathf.Min(Mathf.CeilToInt(CalculateLOD(ViewDistance, HighLodDistance)), LOD.Length);
            MaxShadowLOD = Mathf.Min(Mathf.CeilToInt(CalculateLOD(_shadowDistance, HighLodDistance)), LODLength);
            MaxSubMeshCount = 0;
            //If there's an LOD group
            if (LodGroups != null)
            {
                LOD[] ld = LodGroups.GetLODs();
                LOD = new MeshLOD[ld.Length];
                for (int i = 0; i < ld.Length; i++)
                {
                    Mesh msh = ld[i].renderers[0].GetComponent<MeshFilter>().sharedMesh;
                    Material[] mts = ld[i].renderers[0].sharedMaterials;
                    LOD[i] = new MeshLOD(msh, mts);
                }
            }

            for (int i = 0; i < LOD.Length; i++)
            {
                if(LOD[i].valid){
                    MaxSubMeshCount = Mathf.Max( LOD[i].mesh.subMeshCount, MaxSubMeshCount);
                }
            }

            //Mesh data
            List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments = new List<GraphicsBuffer.IndirectDrawIndexedArgs>();
            for (int i = 0; i < LOD.Length; i++)
            {
                MeshLOD lod = LOD[i];
                arguments.AddRange(lod.InitializeMeshLOD(painter, MaxSubMeshCount, this.name));
            }

            if(LOD.Length > 0 && LOD[0].mesh != null){
                Bounds meshBounds = LOD[0].mesh.bounds;
                float maxExtent = Mathf.Max(meshBounds.extents.x,meshBounds.extents.z);
                float verticalSize = meshBounds.extents.y*MaximumScale;
                MinMeshBounds = meshBounds.center-new Vector3(maxExtent,verticalSize,maxExtent);
                MaxMeshBounds = meshBounds.center + new Vector3(maxExtent,verticalSize,maxExtent); 
            }   
            return arguments;
        }

        public void GizmosDrawDistances(Vector3 position){
            if(DrawDistances){
                #if UNITY_EDITOR
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(position, Vector3.up, ViewDistance);
                Handles.color = Color.red;
                Handles.DrawWireDisc(position, Vector3.up, _shadowDistance);
                Handles.color = Color.green;
                Handles.DrawWireDisc(position, Vector3.up, HighLodDistance);
                #endif
            }
        }
    

        public IRenderVegetation GetRenderer(IPaintTerrain painter, IGenerate<TextureResult> TextureMaker,PointStore store, VegetationSettings settings, 
            List<Camera> cameras, RenderTexture[] depthtextures, Transform colliderParent)
        {
            return new Vegetation_GPUInstancer(this, painter, TextureMaker, store, settings, cameras, depthtextures, colliderParent);
        }

        public ICompact GetCompactor(VegetationSettings settings, int TotalChunksCount, List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments)
        {
            return new CountCompactDraw(this, settings, TotalChunksCount, arguments, LOD, CastShadows, MaxShadowLOD, LODLength, MaxSubMeshCount, ShadowsLODOffset);
        }

        public override object Preview()
        {
            if(LOD.Length <= 0)
                return null;
            return LOD[0].mesh;
        }

                
        private float CalculateLOD(in float dist, in float LodDistance){
            return Mathf.Log((dist + LodDistance) / (LodDistance), 2);
        }
    }
}