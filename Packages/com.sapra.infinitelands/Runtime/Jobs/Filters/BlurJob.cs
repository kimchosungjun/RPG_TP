using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct BlurJob<T> : IJobFor where T : ChannelBlurFast
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;
        
        IndexAndResolution target;
        IndexAndResolution current;

        int arrayLength;
        
        int EffectSize;
        float ExtraStrength;
        float averageMax;
        public void Execute(int x)
        {   
            float CT = 0;
            for(int y = 0; y < target.Resolution+1; y++){
                int2 vector = default(T).Flip(int2(x, y));
                float average = default(T).BlurValue(globalMap, vector, target,current, EffectSize, ExtraStrength, arrayLength, averageMax, ref CT);
                int index = MapTools.VectorToIndex(vector, target.Resolution);
                globalMap[target.Index * arrayLength + index] = average;
            }
        }

        public static JobHandle SchedulleParallel(NativeArray<float> globalMap, int EffectSize, float ExtraStrength, float averageMax,
            IndexAndResolution target, IndexAndResolution current, int arrayLength, JobHandle dependency) => new BlurJob<T>(){
                globalMap = globalMap,
                EffectSize = EffectSize,
                ExtraStrength = ExtraStrength,
                target = target,
                current = current,
                arrayLength = arrayLength,
                averageMax = averageMax,
            }.ScheduleParallel(target.Resolution+1, 32, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct BlurJobMasked<T> : IJobFor where T : ChannelBlurFast
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;
        
        IndexAndResolution target;
        IndexAndResolution current;
        IndexAndResolution original;
        IndexAndResolution mask;

        int arrayLength;
        
        int EffectSize;
        float ExtraStrength;
        float averageMax;
        public void Execute(int x)
        {           
            float CT = 0;
            for(int y = 0; y < target.Resolution+1; y++){
                int2 vector = default(T).Flip(int2(x, y));

                float average = default(T).BlurValue(globalMap, vector, target,current, EffectSize, ExtraStrength, arrayLength, averageMax, ref CT);
                int index = MapTools.VectorToIndex(vector, target.Resolution);

                int maskIndex = MapTools.RemapIndex(index, target.Resolution, mask.Resolution);
                int originalIndex = MapTools.RemapIndex(index, target.Resolution, original.Resolution);

                float maskValue = globalMap[mask.Index * arrayLength + maskIndex];
                float originalValue = globalMap[original.Index * arrayLength + originalIndex]; 
                globalMap[target.Index * arrayLength + index] = lerp(originalValue, average,maskValue);
            }
        }

        public static JobHandle SchedulleParallel(NativeArray<float> globalMap, int EffectSize, float ExtraStrength, float averageMax,
            IndexAndResolution target, IndexAndResolution current, IndexAndResolution mask,IndexAndResolution original, int arrayLength, JobHandle dependency) => new BlurJobMasked<T>(){
                globalMap = globalMap,
                EffectSize = EffectSize,
                ExtraStrength = ExtraStrength,
                target = target,
                current = current,
                arrayLength = arrayLength,
                averageMax = averageMax,
                mask = mask,
                original = original,
            }.ScheduleParallel(target.Resolution+1, 32, dependency);
    }


    public interface ChannelBlurFast{
        public float BlurValue(NativeArray<float> globalMap, int2 vector, IndexAndResolution target, IndexAndResolution current, 
            int EffectSize, float ExtraStrength, int arrayLength, float averageMax, ref float currentTotal);
        public int2 Flip(int2 val);
    }
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct BlurItJobX : ChannelBlurFast{
        public float BlurValue(NativeArray<float> globalMap, int2 vector, IndexAndResolution target, IndexAndResolution current, 
            int EffectSize, float ExtraStrength, int arrayLength, float averageMax, ref float currentTotal){
            if(vector.y == 0)
            {
                currentTotal = 0;
                for(int z = -EffectSize; z <= EffectSize; z++){
                    int nY = vector.y+z;
                    currentTotal += getValueAtIndex(globalMap,int2(vector.x,nY),target, current, arrayLength);
                    int sZ = (int)sign(z);
                    if(z == -EffectSize || z == EffectSize)
                    {
                        float next = getValueAtIndex(globalMap,int2(vector.x,nY+sZ),target, current, arrayLength);
                        currentTotal += next*ExtraStrength;
                    }
                }
            }
            else{
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x,vector.y-EffectSize-2),target, current, arrayLength)*ExtraStrength;
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x,vector.y-EffectSize-1),target, current, arrayLength);
                currentTotal += getValueAtIndex(globalMap,int2(vector.x,vector.y-EffectSize-1),target, current, arrayLength)*ExtraStrength;
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x,vector.y+EffectSize),target, current, arrayLength)*ExtraStrength;
                currentTotal += getValueAtIndex(globalMap,int2(vector.x,vector.y+EffectSize),target, current, arrayLength);
                currentTotal += getValueAtIndex(globalMap,int2(vector.x,vector.y+EffectSize+1),target, current, arrayLength)*ExtraStrength;
            }
            
            return currentTotal/averageMax;
        }
                
        static float getValueAtIndex(NativeArray<float> globalMap, int2 coord, IndexAndResolution target, IndexAndResolution current, int arrayLenght){
            int index = MapTools.GetFlatIndex(coord, target.Resolution, current.Resolution);
            index = Mathf.Clamp(index, 0, current.Length-1);
            return globalMap[arrayLenght * current.Index + index];
        }

        public int2 Flip(int2 val)
        {
            return val;
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct BlurItJobY : ChannelBlurFast{
        public float BlurValue(NativeArray<float> globalMap, int2 vector, IndexAndResolution target, IndexAndResolution current, int EffectSize, float ExtraStrength, int arrayLength, float averageMax, ref float currentTotal){
            if(vector.x == 0)
            {
                currentTotal = 0;
                for(int w = -EffectSize; w <= EffectSize; w++){
                    int nX = vector.x+w;
                    currentTotal += getValueAtIndex(globalMap,int2(nX,vector.y),target, current, arrayLength);
                    int sW = (int)sign(w);
                    if (w == -EffectSize || w == EffectSize){
                        float next = getValueAtIndex(globalMap,int2(nX+sW,vector.y),target, current, arrayLength);
                        currentTotal += next*ExtraStrength;
                    }
                }
            }
            else{
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x-EffectSize-2, vector.y),target, current, arrayLength)*ExtraStrength;
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x-EffectSize-1, vector.y),target, current, arrayLength);
                currentTotal += getValueAtIndex(globalMap,int2(vector.x-EffectSize-1, vector.y),target, current, arrayLength)*ExtraStrength;
                currentTotal -= getValueAtIndex(globalMap,int2(vector.x+EffectSize, vector.y),target, current, arrayLength)*ExtraStrength;
                currentTotal += getValueAtIndex(globalMap,int2(vector.x+EffectSize, vector.y),target, current, arrayLength);
                currentTotal += getValueAtIndex(globalMap,int2(vector.x+EffectSize+1, vector.y),target, current, arrayLength)*ExtraStrength;
            }

            return currentTotal/averageMax;
        }
                
        static float getValueAtIndex(NativeArray<float> globalMap, int2 coord, IndexAndResolution target, IndexAndResolution current, int arrayLenght){
            int index = MapTools.GetFlatIndex(coord, target.Resolution, current.Resolution);
            index = Mathf.Clamp(index, 0, current.Length-1);
            return globalMap[arrayLenght * current.Index + index];
        }

        public int2 Flip(int2 val)
        {
            return new int2(val.y, val.x);
        }
    }
}