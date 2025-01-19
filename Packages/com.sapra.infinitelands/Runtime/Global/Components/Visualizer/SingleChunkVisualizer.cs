using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using Unity.Jobs;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace sapra.InfiniteLands{  

    [ExecuteInEditMode]
    public class SingleChunkVisualizer : MonoBehaviour, IVisualizeTerrain, IControlMatrices
    {
        [SerializeField] private MeshSettings meshSettings = MeshSettings.Default;
        [SerializeField] private TerrainGenerator terrainGenerator;       

        [Header("Results")]         
        private DataManager _manager;
        private IndexManager _indexManager;
        private IGraph _previousGenerator;

        private System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();
        private ChunkData _worldData = null;
        public IRenderChunk chunkLoader;

        [Header("Flags")]
        private bool _generating = false;
        private bool _reTrigger = false;
        private Vector3 _previousPosition;

        public Action<ChunkData> onProcessDone { get; set; }
        public Action<ChunkData> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload { get; set; }
        public MeshSettings settings => meshSettings;
        public IGraph graph{get; private set;}
        public Vector2 localGridOffset{ get{
                Vector3 point = worldToLocalMatrix.MultiplyPoint(transform.position);
                return new Vector2(point.x, point.z);
            }
        } 
        public Matrix4x4 localToWorldMatrix => Matrix4x4.TRS(
            transform.localToWorldMatrix.MultiplyPoint(-new Vector3(transform.position.x,0, transform.position.z)), 
            transform.rotation, 
            transform.localScale);

        public Matrix4x4 worldToLocalMatrix => Matrix4x4.Inverse(localToWorldMatrix);
        
        private bool CanGenerate => graph != null && graph.ValidOutput;

        public bool DrawGizmos => DrawChunks;
        [SerializeField] private bool showTimings;
        [SerializeField] private bool DrawChunks;
        private void Start()
        {
            transform.localScale = Vector3.one;
            _previousPosition = transform.position;
            _previousGenerator = graph;

            if(_generating)
                ApplyResults(false);
            
            ForceGeneration(true);
        }
        
        #if UNITY_EDITOR
        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += ForceApply;

            EditorSceneManager.sceneSaved -= RegenAfterSceneSaved;
            EditorSceneManager.sceneSaved += RegenAfterSceneSaved;
            InstaUpdate();
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= ForceApply;
            EditorApplication.update -= UpdateResults;
            EditorApplication.delayCall -= DelayedCall;

            JobHandle current = _worldData != null ? _worldData.handle : default;
            _manager?.Dispose(current);
        }
        #if UNITY_EDITOR
        private void RegenAfterSceneSaved(UnityEngine.SceneManagement.Scene scene){
            InstaUpdate();
        }
        #endif
        #endif

        public void ForceGeneration(bool instantGen)
        {            
            if (_generating) {
                if(!instantGen)
                {
                    _reTrigger = true;
                    return;
                }
                else
                    ApplyResults(false);
            }

            graph?.ValidationCheck();

            if (!CanGenerate)
                return;

            _watch = new System.Diagnostics.Stopwatch();
            _watch.Start();
            
            SubscribeAutoUpdate();
            GenerateMesh();
            if(instantGen){
                ApplyResults(true);
            }
            #if UNITY_EDITOR
            else
                EditorApplication.update += UpdateResults;
            #endif
        }
            
        private void GenerateMesh()
        {                
            _generating = true;
            //chunkLoader?.DisableChunk(false);
          
            Vector3 position = worldToLocalMatrix.MultiplyPoint(transform.position);
            Vector2 simplePos = new Vector2(position.x, position.z);
            Vector2Int coord = Vector2Int.FloorToInt(simplePos / meshSettings.MeshScale);
            Vector3Int id = new Vector3Int(coord.x, coord.y, 0);

            TerrainConfig configuration = new TerrainConfig(id, graph.MinMaxHeight,  transform.up, meshSettings.MeshScale, new Vector3(transform.position.x,0, transform.position.z));

            //_manager?.Dispose();
            if(_manager == null)
                _manager = new DataManager();
            
            _indexManager = new IndexManager();

            if(chunkLoader != null){
                chunkLoader.Reuse(configuration, meshSettings);
            }
               
            RequestMesh(configuration);

            GraphSettingsController.ChangeValueSettings(meshSettings.MeshScale, new Vector2(configuration.Position.x, configuration.Position.z), meshSettings.Seed);
            graph.Initialize();            
        }


        private void ApplyResults(bool inform)
        {
            if(!CanGenerate || _worldData == null)
                return;

            if(inform){
                onProcessRemoved?.Invoke(_worldData);
                onReload?.Invoke(graph, settings);
            }
            if (showTimings)
                Debug.Log("<-------------- NEW MESH ---------------->");
            
            _worldData.Complete(); 
            _worldData.InstantGeneration = true;
            
            _watch.Stop();
            if (showTimings)
            {
                Debug.Log(string.Format("Chunk generated in: {0} ms, {1} ticks", _watch.ElapsedMilliseconds,
                    _watch.ElapsedTicks));
                Debug.Log("<!-------------- THE END ----------------!>");
            }

            if(inform)
                onProcessDone?.Invoke(_worldData);

            _worldData.Return();
            _generating = false;
        }

        void LateUpdate()
        {
/*             if(chunkLoader != null){
                chunkLoader.VisibilityCheck(transform.position, default, meshSettings.MeshScale*2);
            } */
            if (transform.position != _previousPosition && graph != null && graph._autoUpdate)
            {
                ForceGeneration(false);
                _previousPosition = transform.position;
            }
        }

        public void ChangeGenerator(IGraph generator)
        {
            graph = generator;
            terrainGenerator = generator as TerrainGenerator;
        }
        private void OnValidate()
        {
            ChangeGenerator(terrainGenerator);

            #if UNITY_EDITOR
            if(_previousGenerator != graph){
                if(_previousGenerator != null){
                    _previousGenerator.OnValuesChanged -= InstaUpdate;
                }
                InstaUpdate();
            }
            else{
                if (graph != null && graph._autoUpdate && !_generating){
                    InstaUpdate();
                }
            }
            _previousGenerator = graph;
            #endif
        }
        private void SubscribeAutoUpdate()
        {
            if (graph != null)
            {
                graph.OnValuesChanged -= InstaUpdate;
                graph.OnValuesChanged += InstaUpdate;
            }

            chunkLoader = GetComponentInChildren<IRenderChunk>();
            if(chunkLoader == null || chunkLoader.gameObject == null || chunkLoader.gameObject.Equals(this.gameObject)){
                Debug.LogWarning("There are no Chunk Control for the mesh to be rendered at. Spawning default Single Chunk");
                GameObject defaultSingle = new GameObject("Single Chunk");
                defaultSingle.transform.SetParent(this.transform);
                defaultSingle.transform.localPosition = Vector3.zero;
                defaultSingle.transform.localRotation = Quaternion.identity;
                defaultSingle.transform.localScale = Vector3.one;

                chunkLoader = defaultSingle.AddComponent<SingleChunk>();
            }
            
/*             if(chunkLoader == null) 
                chunkLoader = gameObject.AddComponent<SingleChunk>(); */
        }

        private void ForceApply(){
            if(_generating)
                ApplyResults(false);
        }

        private void InstaUpdate()
        {
            #if UNITY_EDITOR
            EditorApplication.delayCall -= DelayedCall;
            EditorApplication.delayCall += DelayedCall;
            #endif
        }

        void DelayedCall(){
            if (this != null)
            {
                ForceGeneration(false);
            }
        }

        public void UpdateResults(){
            if(_worldData == null)
                return;

            if(_worldData.IsCompleted && _generating && this != null && this.isActiveAndEnabled){
                ApplyResults(true);
                if(_reTrigger){
                    _reTrigger = false;
                    InstaUpdate();
                } 
                #if UNITY_EDITOR
                EditorApplication.update -= UpdateResults;
                #endif
            }
        }

        public void RequestMesh(TerrainConfig config)
        {
            var generator = new WorldGenerator(graph);
            _worldData = generator.GenerateWorld(config, meshSettings, _manager, _indexManager);
        }

        public void UnrequestMesh(TerrainConfig config, ChunkData chunkData)
        {
            if(chunkData != null){
                chunkData.Return();
                if(chunkData.terrainConfig.ID.Equals(_worldData.terrainConfig.ID))
                    _worldData = null;
            }            
        }

        public void DisableChunk(IRenderChunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        public IRenderChunk GenerateChunk(Vector3Int ID)
        {
            Debug.LogWarning("Single Chunk visualizer doesn't generate more chunks");
            return null;
        }
    }
}
