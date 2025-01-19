using System;
using Unity.Jobs;
using UnityEngine;

using Unity.Collections;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    public abstract class HeightDataGenerator : InfiniteLandsNode, IGive<HeightData>, IHavePreview
    {
        public virtual Vector2 minMaxValue => GetMinMaxValue();
        protected abstract Vector2 GetMinMaxValue();
        protected HeightGenerationResult GetIndex(GenerationSettings settings){
            HeightGenerationResult ind = settings.indices.GetIndices(this, settings.BranchGUID, out bool generated);
            if(generated)
                Debug.LogErrorFormat("Missing {0}", guid);
            return UpdateIndices(ind, settings);
        }

        public virtual HeightData RequestHeight(GenerationSettings settings){
            var ind = GetIndex(settings);
            return RequestHeight(ind, settings);
        }

        public virtual IndexAndResolution RequestIndex(GenerationSettings settings){
            var ind = GetIndex(settings);
            return ind.JobReady;
        }
        public abstract HeightData RequestHeight(HeightGenerationResult ind, GenerationSettings settings);

        public HeightData RequestNormal(GenerationSettings settings, out NativeArray<float3> normalMap, out HeightData heightData)
        {
            HeightGenerationResult ind = settings.indices.GetIndices(this, settings.BranchGUID, out _);
            heightData = RequestHeight(ind, settings);

            if(!ind.NormalMapGenerated){
                IndexAndResolution real = heightData.indexData;
                ind.normalIndex = new IndexAndResolution(0, MapTools.IncreaseResolution(real.Resolution,-1));
                ind.normalMap = settings.manager.GetReturnableArray<float3>(settings.terrain.ID, ind.normalIndex.Length);  
                ind.jobHandleNormal = GenerateNormalMap.ScheduleParallel(settings.points, ind.normalMap, settings.globalMap, ind.normalIndex, real, 
                    settings.pointsLength, settings.meshSettings.Resolution, settings.meshSettings.MeshScale, heightData.jobHandle);
                ind.NormalMapGenerated = true;
            }
            
            normalMap = ind.normalMap;
            return ind.CachedNormal;
        }      
        
        public int PrepareNode(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid, bool normalMap = false){
            int newResolution = resolution;
            try{
                HeightGenerationResult request = manager.GetIndices(this, requestGuid, out _);

                if(request.HeightIndex < 0){
                    request.HeightIndex = currentCount;
                    currentCount++;
                    currentCount = IncreaseNodeIndices(currentCount);
                }
                else
                    currentCount = Mathf.Max(currentCount, request.HeightIndex+1);

                if(normalMap)
                    resolution = MapTools.IncreaseResolution(resolution, 1);
                
                if(resolution > request.OutputResolution){
                    request.OutputResolution = resolution;
                    if(isValid){
                        request.RequestedResolution = PrepareSubNodes(manager,ref currentCount, resolution, ratio, requestGuid);
                        
                    }
                }

                newResolution = request.RequestedResolution;
            }
            catch(Exception e){
                Exception ne = new Exception(string.Format("Error initializing {0}: {1}", this.name, e.Message), e);
                Debug.LogException(ne);
            }

            return newResolution;
        }
        protected virtual int IncreaseNodeIndices(int currentIndex){return currentIndex;}
        protected virtual int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid){return resolution;}

        #region Helper
        protected virtual HeightGenerationResult UpdateIndices(HeightGenerationResult indicesReady, GenerationSettings settings) => indicesReady;
        #endregion


        #region ImageGeneration
        [SerializeField][HideInInspector] bool generateTexture;
        public Action<bool> OnImageUpdated { get; set; }
        private BurstTexture texture;

        public object GetTemporalTexture() => texture.ApplyTexture();
        public bool ShowPreview() => generateTexture;
        public void GenerateTexture(GenerationSettings settings, IBurstTexturePool TexturePool)
        {
            if(!isValid){
                TogglePreview(false);
                return;
            }

            try{
                HeightData afterDone = RequestHeight(settings);
                CreatePreview(afterDone, settings, TexturePool, Application.isPlaying?Destroy:DestroyImmediate);
            }
            catch(Exception){
                Debug.LogWarningFormat("Error loading up the input channels of {0}. Proably missing some inputs.", this.name);
            }
        }
        public void TogglePreview(bool value, bool forcedUpdate = false)
        {
            if(value.Equals(generateTexture))
                return;

            generateTexture = value;
            if(!value)
                OnImageUpdated?.Invoke(value);
            if(forcedUpdate)
                OnValuesUpdated?.Invoke();
        }

        public void CreatePreview(HeightData input, GenerationSettings settings, IBurstTexturePool TexturePool, Action<Texture2D> DestroyMethod)
        {
            texture = TexturePool.GetTexture(this.name, FilterMode.Point);
            NativeArray<Color32> raw = texture.GetRawData<Color32>();
            JobHandle textureJob = MTJGeneral.ScheduleParallel(raw, settings.globalMap, GetMinMaxValue(),
                input.indexData, settings.pointsLength, TexturePool.GetTextureResolution(), input.jobHandle);
            
            textureJob.Complete();
            texture.ApplyTexture();
            OnImageUpdated?.Invoke(true);
        }

        public bool PreviewButtonEnabled()=>true;
        #endregion
    }
}