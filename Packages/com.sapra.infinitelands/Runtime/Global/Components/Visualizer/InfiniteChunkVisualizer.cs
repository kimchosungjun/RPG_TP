using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using sapra.InfiniteLands.NaughtyAttributes;
using Unity.Jobs;
using Unity.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sapra.InfiniteLands{  

    public class InfiniteChunkVisualizer : MonoBehaviour, IVisualizeTerrain, IControlMatrices
    {
        public enum GenerationMode{FarFirst, CloseFirst}

        [SerializeField] private MeshSettings meshSettings = MeshSettings.Default;
        [SerializeField] private TerrainGenerator terrainGenerator;

        [SerializeField] private Camera Camera;
        [SerializeField] private Transform Player;
        public GameObject ChunkPrefab;
        public GenerationMode _GenerationMode;

        [Header("World generation")] [Tooltip("Distance to stop generating chunks")]
        public bool DecimatedForNonCloseLOD;
        [AllowNesting] [ShowIf("DecimatedForNonCloseLOD")][Min(1)] public int DecimatedLOD = 2;

        public float GenerationDistance = 1000;
        [Min(1)] public int MaxChunksPerFrame = 1;
        [Tooltip("When to start cleaning the data generated to free memory")]
        public string GroundLayer = "Default";       

        private Transform DisabledParent;
        private Transform EnabledParent;

        private InfiniteSettings infiniteSettings;
        private float MaxScale;

        private Dictionary<Vector2Int, IRenderChunk> OverviewChunks = new();
        private List<Vector2Int> VisibleChunks = new();
        private List<IRenderChunk> DisabledChunks = new();
        private Dictionary<int, List<TerrainConfig>> MeshRequests = new();
        private int CurrentBiggest;

        private List<ChunkData> GenerationCalls = new List<ChunkData>();

        private FloatingOrigin floatingOrigin;

        private DataManager _manager;
        private IndexManager _indexManager;

        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private WorldGenerator generator;

        public Action<ChunkData> onProcessDone { get; set; }
        public Action<ChunkData> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload { get; set; }
        protected bool CanGenerate => graph != null && graph.ValidOutput;
        protected HashSet<Vector3Int> UnnecessaryChunks = new();
        public ISetupChunkHirearchy ChunkHirearchy;

        public bool DrawGizmos => DrawChunks;

        [SerializeField] private bool DrawChunks;
        [SerializeField] private bool DrawDistances;

        public void ForceGeneration(bool instanUpdate){}

        public MeshSettings settings => meshSettings;
        public IGraph graph{get; private set;}
        
        public Vector2 localGridOffset => new Vector2(meshSettings.MeshScale / 2, meshSettings.MeshScale / 2);
        public Matrix4x4 localToWorldMatrix{get; private set;}
        public Matrix4x4 worldToLocalMatrix{get; private set;}
        private void Start()
        {   
            localToWorldMatrix = transform.localToWorldMatrix;
            worldToLocalMatrix = transform.worldToLocalMatrix;
            ChangeGenerator(terrainGenerator);
            floatingOrigin = GetComponent<FloatingOrigin>();
            if(floatingOrigin != null)
                floatingOrigin.OnOriginMove += OnOriginShift;
            ReloadChunk();

            transform.localScale = Vector3.one;
            DisabledParent = new GameObject("Disabled Chunks").transform;
            DisabledParent.SetParent(this.transform);
            EnabledParent = new GameObject("Enabled Chunks").transform;
            EnabledParent.SetParent(this.transform);

            if(graph == null)
                return;
                
            graph.ValidationCheck();
            
            if(!CanGenerate)
                return;

            _manager = new DataManager();
            _indexManager = new IndexManager();

            graph.Initialize();
            generator = new WorldGenerator(graph);
            infiniteSettings = ChunkHirearchy.GetInfiniteSettings(meshSettings, GenerationDistance);
            MaxScale = ChunkHirearchy.GetMeshSettingsFromID(meshSettings, new Vector3Int(0,0, infiniteSettings.LODLevels-1)).MeshScale;
            onReload?.Invoke(graph, meshSettings);

            watch.Start();
        }

        private void OnOriginShift(Vector3Double newOrigin, Vector3Double previousOrigin){
            Matrix4x4 copy = worldToLocalMatrix;
            Vector3 worldOrigin = transform.worldToLocalMatrix.MultiplyPoint(newOrigin);
            copy.SetColumn(3, new Vector4(worldOrigin.x,worldOrigin.y,worldOrigin.z,1));

            worldToLocalMatrix = copy;
            localToWorldMatrix = worldToLocalMatrix.inverse;
        }  
        private void ReloadChunk(){
            var targetObject = ChunkPrefab;
            if(targetObject == null || targetObject.GetComponent<IRenderChunk>() == null){
                targetObject = GetComponentInChildren<IRenderChunk>()?.gameObject;
                if(targetObject != null && targetObject.Equals(this.gameObject))
                    targetObject = null;
            }
            
            if(targetObject == null || targetObject.GetComponent<IRenderChunk>() == null)
            {
                Debug.LogWarningFormat("There's no gameObject assigned to {0} or no {1} inside the prefab. Using the default {2}", 
                    nameof(ChunkPrefab), typeof(IRenderChunk).ToString(), typeof(QuadChunk).ToString());
                targetObject = new GameObject("Temporal QuadChunk");
                targetObject.transform.SetParent(this.transform);
                targetObject.AddComponent<QuadChunk>();
                if(floatingOrigin)
                    targetObject.gameObject.AddComponent<FloatingPoint>();     
            }

            ChunkHirearchy = targetObject.GetComponent<ISetupChunkHirearchy>();
            ChunkPrefab = targetObject;

            if(floatingOrigin){
                var floatingPoint = ChunkPrefab.GetComponent<FloatingPoint>();
                if(floatingPoint == null)
                    Debug.LogWarningFormat("{0} doesn't have a Floating Point component, but {1} has {2}. This might bring unexpected results", 
                        ChunkPrefab.name, typeof(InfiniteChunkVisualizer).Name, typeof(FloatingOrigin).Name);
            }
  
        }
        private void OnValidate()
        {        
            ChangeGenerator(terrainGenerator);
            if(ChunkHirearchy != null)
                infiniteSettings = ChunkHirearchy.GetInfiniteSettings(meshSettings, GenerationDistance);
        }
        public void ChangeGenerator(IGraph generator)
        {
            graph = generator;
        }
        private void OnDisable()
        {        
/*             foreach (KeyValuePair<Vector2Int, IRenderChunk> world in OverviewChunks)
            {
                world.Value.DisableChunk();
            } */
            NativeArray<JobHandle> tempJobs = new NativeArray<JobHandle>(GenerationCalls.Select(a => a.handle).ToArray(), Allocator.Temp);
            JobHandle currentJob = JobHandle.CombineDependencies(tempJobs);
            tempJobs.Dispose();
            
            if(_manager != null){
                _manager.Dispose(currentJob);
            }

            if(floatingOrigin != null)
                floatingOrigin.OnOriginMove -= OnOriginShift; 
        }
        
        private void AdaptiveDestroy(UnityEngine.Object obj) {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        // Update is called once per frame
        private void Update()
        {
            if(!CanGenerate)
                return;
                
            UpdateVisibleChunks();
            UpdateScheduledJobs();
            CheckFinishedJobs();
            if (GenerationCalls.Count <= 0)
            {
                watch.Stop();
            }
        }
        #region Chunk Generation

        private void UpdateVisibleChunks()
        {
            Plane[] planes = GlobalHelper.GetFrustrumPlanes(Camera, GenerationDistance);
            for(int i = 0; i < planes.Length; i++){
                planes[i] = worldToLocalMatrix.TransformPlane(planes[i]);
            }

            Vector3 fullPosition = (Player != null ? Player.position : Vector3.zero);

            Vector3 position = worldToLocalMatrix.MultiplyPoint(fullPosition);

            int currentChunkCoordX = Mathf.FloorToInt(position.x / (MaxScale));
            int currentChunkCoordY = Mathf.FloorToInt(position.z / (MaxScale));

            List<Vector2Int> ToDisable = new List<Vector2Int>(VisibleChunks);
            for (int yOffset = -infiniteSettings.VisibleChunks; yOffset <= infiniteSettings.VisibleChunks; yOffset++)
            {
                for (int xOffset = -infiniteSettings.VisibleChunks; xOffset <= infiniteSettings.VisibleChunks; xOffset++)
                {
                    Vector2Int flatID = new Vector2Int(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    Vector3Int ID = new Vector3Int(flatID.x,flatID.y, infiniteSettings.LODLevels-1);
                    IRenderChunk generator;
                    if (OverviewChunks.TryGetValue(flatID, out generator))                    
                        generator.VisibilityCheck(position, planes, GenerationDistance);
                    else{
                        generator = GenerateChunk(ID);
                        OverviewChunks.Add(flatID, generator);
                    }
       
                    if (!VisibleChunks.Contains(flatID))
                        VisibleChunks.Add(flatID);
                    else
                        ToDisable.Remove(flatID);

                }
            }


            foreach (Vector2Int coord in ToDisable)
            {
                if (OverviewChunks.TryGetValue(coord, out IRenderChunk generator))
                {   
                    VisibleChunks.Remove(coord);
                    OverviewChunks.Remove(coord);
                    DisableChunk(generator);
                }
            }
        }

        public IRenderChunk GenerateChunk(Vector3Int ID)
        {
            IRenderChunk chunk;
            GameObject terrain;
            if (DisabledChunks.Count > 0)
            {
                chunk = DisabledChunks[0];
                terrain = chunk.gameObject;
                DisabledChunks.RemoveAt(0);
                terrain.SetActive(true);
            }
            else
            {
                terrain = Instantiate(ChunkPrefab, EnabledParent);   
                terrain.layer = LayerMask.NameToLayer(GroundLayer);
                chunk = terrain.GetComponent<IRenderChunk>();       
            }
            
            MeshSettings settings = ChunkHirearchy.GetMeshSettingsFromID(meshSettings, ID);
            TerrainConfig config = new TerrainConfig(ID, graph.MinMaxHeight, transform.up, settings.MeshScale);
            terrain.name = ID.ToString();
            terrain.transform.SetParent(EnabledParent);
            terrain.transform.rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));
            terrain.transform.position = localToWorldMatrix.MultiplyPoint(config.Position);
            terrain.transform.localScale = Vector3.one;
            chunk.Reuse(config, settings);
            return chunk;
        }

        public void DisableChunk(IRenderChunk chunk)
        {
            chunk.gameObject.SetActive(false);
            if(chunk.gameObject.activeInHierarchy)
                chunk.gameObject.transform.SetParent(DisabledParent);
            DisabledChunks.Add(chunk);
        }

        #endregion

        #region Mesh Generation

        public void RequestMesh(TerrainConfig config)
        {
            if(!MeshRequests.TryGetValue(config.ID.z, out List<TerrainConfig> currentList)){
                currentList = new List<TerrainConfig>();
                MeshRequests.Add(config.ID.z, currentList);
            }

            currentList.Add(config);
            switch (_GenerationMode)
            {   
                case GenerationMode.FarFirst:
                    CurrentBiggest = Mathf.Max(CurrentBiggest, config.ID.z);
                    break;
                default:
                    CurrentBiggest = Mathf.Min(CurrentBiggest, config.ID.z);
                    break;
            }
        }

        public void UnrequestMesh(TerrainConfig config, ChunkData data)
        {
            if(MeshRequests.TryGetValue(config.ID.z, out List<TerrainConfig> currentList)){
                bool isItScheduled = currentList.Contains(config);
                if (isItScheduled){
                    currentList.Remove(config);
                    return;
                }
            }
            bool isBeingProcessed = GenerationCalls.Any(a => a.terrainConfig.ID.Equals(config.ID));
            if(isBeingProcessed)
                UnnecessaryChunks.Add(config.ID);

            if(data != null)
                onProcessRemoved?.Invoke(data);
        }

        MeshSettings[] SettingsFromRequests(List<TerrainConfig> requests)
        {
            MeshSettings[] settings = new MeshSettings[requests.Count];
            for (int i = 0; i < requests.Count; i++)
            {
                Vector3Int ID = requests[i].ID;
                MeshSettings selected = ChunkHirearchy.GetMeshSettingsFromID(meshSettings, ID);
                MeshSettings.MeshType target = ID.z >= DecimatedLOD ? MeshSettings.MeshType.Decimated : selected.meshType;
                selected.meshType = DecimatedForNonCloseLOD ? target : selected.meshType;
                settings[i] = selected;
            }

            return settings;
        }

        private List<TerrainConfig> GetCurrentRequests(){
            int defaultValue;
            int startChecking = CurrentBiggest;
            switch(_GenerationMode){
                case GenerationMode.FarFirst:
                    defaultValue = -1;
                    for(int x = startChecking; x >= 0; x--){
                        if(MeshRequests.TryGetValue(x, out List<TerrainConfig> data)){
                            if(data.Count > 0){
                                CurrentBiggest = x;
                                return data;
                            }
                        }
                    }
                    break;
                default: 
                    defaultValue = infiniteSettings.LODLevels;
                    for(int x = startChecking; x < infiniteSettings.LODLevels; x++){
                        if(MeshRequests.TryGetValue(x, out List<TerrainConfig> data)){
                            if(data.Count > 0){
                                CurrentBiggest = x;
                                return data;
                            }
                        }
                    }
                    break;
            }
            CurrentBiggest = defaultValue;
            return null;
        }

        private void UpdateScheduledJobs()
        {
            if (GenerationCalls.Count >= MaxChunksPerFrame)
                return;

            List<TerrainConfig> TargetRequests = GetCurrentRequests();
            if(TargetRequests == null)
                return;

            int SimulatinousManaging = Mathf.Min(TargetRequests.Count, MaxChunksPerFrame);
            List<TerrainConfig> subset = TargetRequests.GetRange(0, SimulatinousManaging);

            TerrainConfig[] configurations = subset.ToArray();
            ChunkData[] generatedData = new ChunkData[subset.Count];

            MeshSettings[] settings = SettingsFromRequests(subset);
                    
            for (int m = 0; m < subset.Count; m++)
            {
                generatedData[m] = generator.GenerateWorld(configurations[m], settings[m], _manager, _indexManager);
                GenerationCalls.Add(generatedData[m]);
            }

            TargetRequests.RemoveRange(0, SimulatinousManaging);
        }

        private void CompleteChunk(ChunkData chunk){
            chunk.Complete();
            if(UnnecessaryChunks.Contains(chunk.terrainConfig.ID))
                UnnecessaryChunks.Remove(chunk.terrainConfig.ID);
            else
                onProcessDone?.Invoke(chunk);
            chunk.Return();
        }

        private void CheckFinishedJobs()
        {
            if (GenerationCalls.Count > 0)
            {
                for (int i = GenerationCalls.Count - 1; i >= 0; i--)
                {
                    ChunkData call = GenerationCalls[i];
                    if (call.IsCompleted)
                    {
                        CompleteChunk(call);
                        GenerationCalls.RemoveAt(i);
                    }
                }
            }
        }
        #endregion

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(DrawDistances){
                int target = CurrentBiggest;
/*                 for(int i = 0; i < CurrentBiggest; i++)
                {
                    float distance = 
                    float lodFraction = (float)(target-i)/target;
                    Handles.color = Color.Lerp(Color.blue, Color.red, lodFraction);
                    Handles.DrawWireDisc(Player.position,Vector3.up, distance);
                    distance /= 2;
                }
                Handles.color = Color.green; */
                Handles.DrawWireDisc(Player.position,Vector3.up, GenerationDistance);
                GetTriangles(GetFrustumCorners(Camera));
                
            }    
        }
        struct Triangle{
            public Vector3 C1;
            public Vector3 C2;
            public Vector3 C3;
            public Triangle(Vector3 c1, Vector3 c2, Vector3 c3) { 
                C1 = c1;
                C2 = c2;
                C3 = c3;

                    
                Debug.DrawLine(c1, c2, Color.red);
                Debug.DrawLine(c2, c3, Color.red);
                Debug.DrawLine(c3, c1, Color.red);
            }
        }
        Vector3[] GetFrustumCorners(Camera cam)
        {
            float viewDistance = cam.farClipPlane;
            cam.farClipPlane = GenerationDistance;
            Vector3[] frustumCornersNear = new Vector3[4];

            // Near plane corners
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersNear);

            // Near plane corners in world space
            for (int i = 0; i < 4; i++)
            {
                frustumCornersNear[i] = cam.transform.TransformPoint(frustumCornersNear[i]);
            }

            Vector3[] frustumCornersFar = new Vector3[4];

            // Far plane corners
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersFar);

            // Far plane corners in world space
            for (int i = 0; i < 4; i++)
            {
                frustumCornersFar[i] = cam.transform.TransformPoint(frustumCornersFar[i]);
            }

            Vector3[] concat = frustumCornersNear.Concat(frustumCornersFar).ToArray();
            cam.farClipPlane = viewDistance;
            return concat;
        }
        Triangle[] GetTriangles(Vector3[] frustumCorners)
        {
            // Define frustum triangles for each side
            Triangle[] frustumTriangles = {
                // Left side
                new(frustumCorners[0], frustumCorners[1], frustumCorners[5]),
                new(frustumCorners[0], frustumCorners[5], frustumCorners[4]),
                // Right side
                new(frustumCorners[2], frustumCorners[3], frustumCorners[7]),
                new(frustumCorners[2], frustumCorners[7], frustumCorners[6]),
                // Top side
                new(frustumCorners[1], frustumCorners[2], frustumCorners[6]),
                new(frustumCorners[1], frustumCorners[6], frustumCorners[5]),
                // Bottom side
                new (frustumCorners[0], frustumCorners[3], frustumCorners[7]),
                new (frustumCorners[0], frustumCorners[7], frustumCorners[4]),
                // Near plane
                new (frustumCorners[0], frustumCorners[1], frustumCorners[2]),
                new(frustumCorners[2], frustumCorners[3], frustumCorners[0]),
                // Far plane
                new (frustumCorners[4], frustumCorners[5], frustumCorners[6]),
                new (frustumCorners[6], frustumCorners[7], frustumCorners[4])
            };
            return frustumTriangles;
        }
        #endif

    }
}