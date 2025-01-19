using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace sapra.InfiniteLands{
    public abstract class ChunkProcessor<T> : MonoBehaviour
    {   
        protected IGenerate<T> provider;
        
        protected void AdaptiveDestroy(UnityEngine.Object obj) {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        #if UNITY_EDITOR
        private void DisposeAfterSceneSave(UnityEngine.SceneManagement.Scene scene, string path){
            Dispose();
        }
        #endif
        void OnDisable()
        {
            if(provider != null){
                provider.onProcessDone -= OnProcessAdded;
                provider.onProcessRemoved -= OnProcessRemoved;
                provider.onReload -= DisposeAndInitialize;
            }

            Dispose();
            #if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload -= Dispose;
            #endif
        }
        
        void OnEnable()
        {
            #if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
            EditorSceneManager.sceneSaving -= DisposeAfterSceneSave;
            EditorSceneManager.sceneSaving += DisposeAfterSceneSave;
            #endif
            GetComponentsAndSubscribe();
            
        }

        private void GetComponentsAndSubscribe(){
            provider = GetComponent<IGenerate<T>>();
            if(provider == null){
                Debug.LogWarning(string.Format("Missing a processor of type {0}", typeof(IGenerate<T>).ToString()));
                return;
            }

            provider.onProcessDone -= OnProcessAdded;
            provider.onProcessDone += OnProcessAdded;

            provider.onProcessRemoved -= OnProcessRemoved;
            provider.onProcessRemoved += OnProcessRemoved;

            provider.onReload -= DisposeAndInitialize;
            provider.onReload += DisposeAndInitialize;

            DisposeAndInitialize(provider.graph, provider.settings);
        }

        private void DisposeAndInitialize(IGraph generator, MeshSettings settings){
            Dispose();
            Initialize(generator, settings);
        }

        public abstract void Dispose();
        public abstract void Initialize(IGraph generator, MeshSettings settings);
        protected abstract void OnProcessRemoved(T chunk);
        protected abstract void OnProcessAdded(T chunk);
    }
}