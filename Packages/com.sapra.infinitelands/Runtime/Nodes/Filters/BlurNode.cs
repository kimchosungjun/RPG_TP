using UnityEngine;
using Unity.Jobs;

using System;
namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Blur", type = "Filter", docs ="https://ensapra.com/packages/infinite_lands/nodes/blur.html")]
    public class BlurNode : HeightNodeBase
    {
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator HeightMap;
        [Input(typeof(IGive<HeightData>), true)] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{HeightMap};
        protected override bool ExtraValidationSteps()
        {
            if(Mask != null){
                Mask.ValidationCheck();
                return Mask.isValid;
            }
            return true;
        }

        [Min(0.01f)]public float BlurSize = .01f;
        protected override int IncreaseNodeIndices(int currentIndex)
        {
            return currentIndex +1;
        }
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            float Size = ratio*BlurSize;
            int increasedResolution = MapTools.IncreaseResolution(resolution, Mathf.CeilToInt(Size));
            int maxResolution = HeightMap.PrepareNode(manager,ref currentCount, increasedResolution, ratio, requestGuid);
            if(Mask != null)
                maxResolution = Math.Max(Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid),maxResolution);
            return maxResolution;
        }

        protected override Vector2 GetMinMaxValue()
        {
            if(HeightMap != null)
                return HeightMap.minMaxValue;
            else
                return new Vector2(0,1);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            float Size =  Mathf.Min(settings.ratio*BlurSize, MapTools.MaxIncreaseSize);
            int EffectSize = Mathf.Max(1, Mathf.FloorToInt(Size));
            float ExtraSize = Mathf.Clamp01(Size-EffectSize);
            
            float averageMa = (EffectSize+ExtraSize)*2+1;

            HeightData previousJob = HeightMap.RequestHeight(settings);

            IndexAndResolution current = previousJob.indexData;
            IndexAndResolution channelX = new IndexAndResolution(target.Index+1,current.Resolution);
            
            if(Mask != null){
                HeightData maskJob = Mask.RequestHeight(settings);

                JobHandle onceCompleted = JobHandle.CombineDependencies(maskJob.jobHandle, previousJob.jobHandle);
                JobHandle checkX = BlurJob<BlurItJobX>.SchedulleParallel(settings.globalMap, EffectSize, ExtraSize, averageMa, channelX, current,
                    settings.pointsLength, onceCompleted);

                return BlurJobMasked<BlurItJobY>.SchedulleParallel(settings.globalMap, EffectSize, ExtraSize, averageMa, target, channelX,maskJob.indexData, current,
                    settings.pointsLength, checkX);
            }
            else{
                JobHandle checkX = BlurJob<BlurItJobX>.SchedulleParallel(settings.globalMap, EffectSize, ExtraSize, averageMa, channelX, current,
                    settings.pointsLength, previousJob.jobHandle);

                return BlurJob<BlurItJobY>.SchedulleParallel(settings.globalMap, EffectSize, ExtraSize, averageMa, target, channelX,
                    settings.pointsLength, checkX);
            }

        }
    }
}