using System.Collections.Generic;
using UnityEngine;
using sapra.InfiniteLands.NaughtyAttributes;
using System.Linq;

using UnityEngine.Rendering;
using UnityEditor;

namespace sapra.InfiniteLands
{
    [RequireComponent(typeof(PointStore))]
    [ExecuteAlways]
    public class VegetationRenderer : ChunkProcessor<TextureResult>
    {
        public static int textureIsSetID = Shader.PropertyToID("_DisplacementTextureIsSet");
        public static ComputeShader 
            CalculatePositions,
            VisibilityCheck,
            FillArguments;

        public static LocalKeyword
            CULLING,
            ShadowKeyword;

        public static int 
            CalculatePositionsKernel,
            OriginShiftKernel,
            VisibilityCheckKernel,
            InitialCompactKernel,
            ResetKernel,
            CountKernel,
            SumKernel,
            CompactKernel,
            FillKernel;
        
        [Header("Vegetation configuration")] 
        public LayerMask RenderInLayers = 1;
        [Tooltip("The amount of instances that can be inside 1x1")]
        public int MaxRenderingDistance = -1;
        public int DensityPerSize = 800;
        public bool RenderVegetation = true;
        private bool CullingEnabled = true;

        [Header("Displacement")] 
        public bool DisplaceWithMovement;
        [AllowNesting][ShowIf("DisplaceWithMovement")] public Transform PlayerCenter;
        [AllowNesting][ShowIf("DisplaceWithMovement")] public LayerMask CullMask;
        [AllowNesting][ShowIf("DisplaceWithMovement")] [Min(10)] public float DisplaceDistance = 100;
        [AllowNesting][ShowIf("DisplaceWithMovement")] public bool VisualizeDisplacement;

        private List<Camera> Cameras;
        private RenderTexture[] DepthTextures;
        private MaterialPropertyBlock PropertyBlock;
        private List<IRenderVegetation> VegetationLoaders = new();

        private IPaintTerrain painter;
        private IVisualizeTerrain terrainVisualizer;
        private IControlMatrices matrixControl;

        private PointStore store;
        private FloatingOrigin floatingOrigin;
        private bool Initialized;

        private List<Rigidbody> bodies;
        private ProfilingSampler OriginShiftSampler;

        private List<VegetationSettings> vegSettings = new();

        private void GetShaderData() {
            CalculatePositions = Resources.Load<ComputeShader>("Computes/CalculatePositions");
            FillArguments = Resources.Load<ComputeShader>("Computes/CountCompact");
            VisibilityCheck = Resources.Load<ComputeShader>("Computes/VisibilityCheck");
            CalculatePositionsKernel = CalculatePositions.FindKernel("CalculatePositions");
            OriginShiftKernel = CalculatePositions.FindKernel("OriginShift");
            VisibilityCheckKernel = VisibilityCheck.FindKernel("VisibilityCheck");

            InitialCompactKernel = FillArguments.FindKernel("InitialCompact");
            ResetKernel = FillArguments.FindKernel("Reset");
            CountKernel = FillArguments.FindKernel("Count");
            SumKernel = FillArguments.FindKernel("Sum");
            CompactKernel = FillArguments.FindKernel("Compact");
            FillKernel = FillArguments.FindKernel("FillArguments");

            ShadowKeyword = new LocalKeyword(FillArguments, "SHADOWS");
            CULLING = new LocalKeyword(VisibilityCheck, "CULLING");

            OriginShiftSampler = new ProfilingSampler("Shifting Origin");
        }
        private void OnValidate() {
            if(vegSettings.Count > 0){
                foreach(VegetationSettings seting in vegSettings){
                    seting.UpdateViewDistance(MaxRenderingDistance);
                }
            }
        }
        public override void Dispose()
        {
            if (VegetationLoaders != null)
            {
                foreach (IRenderVegetation vegetation in VegetationLoaders)
                {
                    if(vegetation != null){
                        IHoldVegetation set = vegetation.GetAsset();
                        if(set is UpdateableSO updateable){
                            updateable.OnValuesUpdated -= Reload;
                        }
                        vegetation.Dispose();
                    }
                }
            }
            if(floatingOrigin != null)
                floatingOrigin.OnOriginMove -= OnOriginShift;
            
            VegetationLoaders = null;
            Initialized = false;
        }

        public override void Initialize(IGraph graph, MeshSettings settings)
        {   
            bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None).ToList();

            GetShaderData();
            if(Application.isPlaying)
                CullingEnabled = true;

            store = GetComponent<PointStore>();
            floatingOrigin = GetComponent<FloatingOrigin>();
            if(floatingOrigin != null)
                floatingOrigin.OnOriginMove += OnOriginShift;
            
            painter = GetComponent<IPaintTerrain>();
            terrainVisualizer = GetComponent<IVisualizeTerrain>();
            matrixControl = GetComponent<IControlMatrices>();
            PrepareCamerasAndDepthTextures();
            PrepareAssets(graph, settings);
            PrepareDisplacementTexture();

            PropertyBlock = new MaterialPropertyBlock();
            Initialized = true;
        }

        public void AddNewBody(Rigidbody body){
            if(!bodies.Contains(body))
                bodies.Add(body);
        }

        public void RemoveBody(Rigidbody body){
            if(bodies.Contains(body))
                bodies.Remove(body);
        }

        public void OnOriginShift(Vector3Double newOrigin, Vector3Double previousOrigin){
            if(VegetationLoaders == null || VegetationLoaders.Count <= 0)
                return;
            
            CommandBuffer bf = CommandBufferPool.Get("Vegetation Renderer");
            ComputeShader compute = VegetationRenderer.CalculatePositions;
            int kernel = VegetationRenderer.OriginShiftKernel;
            Vector3 offset = previousOrigin - newOrigin;

            bf.SetComputeVectorParam(compute, "_OriginOffset", offset);
            using(new ProfilingScope(bf, OriginShiftSampler))
            {
                foreach(IRenderVegetation renderer in VegetationLoaders){
                    renderer.OnOriginShift(bf,compute, kernel, offset);
                }
            }
            Graphics.ExecuteCommandBuffer(bf);
            CommandBufferPool.Release(bf);
        }

        private void Reload(){
            Initialize(provider.graph, provider.settings);
        }

        protected override void OnProcessRemoved(TextureResult chunk)
        {   

        }

        protected override void OnProcessAdded(TextureResult chunk)
        {

        }

        #if UNITY_EDITOR
        void ReloadEditorMode(){
            var target = IsGameViewOpenAndFocused() || Application.isPlaying;
            bool shouldSwap = target != CullingEnabled;
            
            CullingEnabled = target;
            if(shouldSwap)
                Initialize(provider.graph, provider.settings);
        }
        #endif

        void PrepareAssets(IGraph generator, MeshSettings settings){
            if(generator == null)
                return;

            if(VegetationLoaders != null && VegetationLoaders.Count > 0){
                foreach(IRenderVegetation loader in VegetationLoaders){
                    IHoldVegetation set = loader.GetAsset();
                    if(set is UpdateableSO updateable){
                        updateable.OnValuesUpdated -= Reload;
                    }
                    loader.Dispose();
                }
            }
            
            IHoldVegetation[] Sets = generator.GetAssets().SelectMany(a => a.assets.OfType<IHoldVegetation>()).ToArray();
            VegetationLoaders = new List<IRenderVegetation>();
            Vector2 localGridOffset = MapTools.GetOffsetInGrid(terrainVisualizer.localGridOffset, settings.MeshScale);
            vegSettings.Clear();
            for (int i = 0; i < Sets.Length; i++)
            {
                IHoldVegetation set = Sets[i];
                if(set.GetDistanceBetweenItems() > 5 && set is UpdateableSO updateable){
                    updateable.OnValuesUpdated += Reload;
                }

                float renderDistance = MaxRenderingDistance >= 0 ? Mathf.Min(set.GetViewDistance(), MaxRenderingDistance) : set.GetViewDistance();
                VegetationSettings vegetationSettings = new VegetationSettings(settings.MeshScale, i, 
                    DensityPerSize, localGridOffset, set.GetDistanceBetweenItems(), renderDistance);
                vegSettings.Add(vegetationSettings);
                IRenderVegetation loader = set.GetRenderer(painter, provider, store, vegetationSettings, Cameras, DepthTextures, transform);
                
                VegetationLoaders.Add(loader);
            }  
        }
        void PrepareCamerasAndDepthTextures(){
            if(CullingEnabled)
                Cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None).Where(a => (a.cullingMask & (RenderInLayers)) != 0 && a.isActiveAndEnabled).ToList();
            #if UNITY_EDITOR
            else
                Cameras = SceneView.GetAllSceneCameras().ToList();
            #endif

            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None).Where(a => a.type == LightType.Directional).ToArray();
            if(lights.Length > 1)
                Debug.LogError("There are too many directional lights on scene, this can return in problems when applying Frustrum Culling to the sahows");
            foreach(Light light in lights){
                var current = light.GetComponent<SetGlobalLightDirection>();
                if(!current)
                    current = light.gameObject.AddComponent<SetGlobalLightDirection>();
            }

            DepthTextures = new RenderTexture[Cameras.Count];
            for (int i = 0; i < Cameras.Count; i++)
            {
                Camera cam = Cameras[i];
                if (!cam.TryGetComponent(out CreateDepthTexture depthCreator))
                {
                    depthCreator = cam.gameObject.AddComponent<CreateDepthTexture>();
                }

                DepthTextures[i] = depthCreator.DepthTexture;
            }
        }
        
        void PrepareDisplacementTexture(){
            DisplaceWithMovement = DisplaceWithMovement && GraphicsSettings.defaultRenderPipeline == null;
            if (DisplaceWithMovement)
            {
                Shader.SetGlobalInt(textureIsSetID, 1);
                if(PlayerCenter == null)    
                    PlayerCenter = this.transform;
                CreateDisplacementTexture Displacer = gameObject.GetComponentInChildren<CreateDisplacementTexture>();
                if (Displacer == null)
                {
                    GameObject DisplacerGO = new GameObject("Vegetation Displacer");
                    DisplacerGO.transform.SetParent(transform);
                    Displacer = DisplacerGO.AddComponent<CreateDisplacementTexture>();
                }
                
                Displacer.Initialize(store, PlayerCenter, CullMask, DisplaceDistance, VisualizeDisplacement);
            }
            else
                Shader.SetGlobalInt(textureIsSetID, 0);

        }

        private void LateUpdate()
        {
            #if UNITY_EDITOR
            if(!Application.isPlaying)
                ReloadEditorMode();
            else
                CullingEnabled = true;
            #endif

            if(!Initialized || !RenderVegetation)
                return;

            if(VegetationLoaders == null)
                return;


            PropertyBlock.Clear();
            foreach (IRenderVegetation vegetation in VegetationLoaders)
            {   
                vegetation.Render(matrixControl, PropertyBlock, CullingEnabled);

                foreach(Rigidbody body in bodies){
                    if(body == null)
                        continue;
                    Vector3 transformedPlayerPosition = matrixControl.worldToLocalMatrix.MultiplyPoint(body.position);
                    vegetation.HandleColliders(transformedPlayerPosition);
                }
            }
        }


        private void OnDrawGizmos()
        {
            if(VegetationLoaders == null || !Initialized)
                return;

            foreach (IRenderVegetation vegetation in VegetationLoaders)
            {
                vegetation.DrawGizmos();
            }
        }

        #if UNITY_EDITOR
        public static bool IsGameViewOpenAndFocused() {
            var windows = Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
            foreach (var window in windows) {
                if (window.GetType().FullName != "UnityEditor.GameView") {
                    continue;
                }

                if (window.hasFocus) {
                    return true;
                }
            }

            return false;
        }
        #endif
    }
}