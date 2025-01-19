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
    public struct CalculateChannels : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;

        [ReadOnly] NativeArray<float3> normalMap;
        IndexAndResolution normalIndex;
        IndexAndResolution target;
        int arrayLenght;

        float4x4 localToWorld;
        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, normalIndex.Resolution);
            float3 normal = normalMap[index];
            float4 inWorldSpace = mul(float4(normal.xyz, 1.0f),localToWorld);
            globalMap[arrayLenght * target.Index + i] = inWorldSpace.x;
            globalMap[arrayLenght * (target.Index+1) + i] = inWorldSpace.z;
        }
        
        public static JobHandle ScheduleParallel(
            NativeArray<float3> normalMap, IndexAndResolution normalIndex,
            NativeArray<float> globalMap, float4x4 localToWorld,
            IndexAndResolution targetIndex,int arrayLenght,
            JobHandle dependency) => new CalculateChannels()
        {
            localToWorld = localToWorld,
            normalMap = normalMap,
            normalIndex = normalIndex,
            target = targetIndex,
            arrayLenght = arrayLenght,
            globalMap = globalMap,
        }.ScheduleParallel(targetIndex.Length, targetIndex.Resolution, dependency);
    }



    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct GetCavityJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;

        IndexAndResolution target;
        IndexAndResolution channelIndex;
        int arrayLenght;

        int EffectSize;
        float ExtraStrength;
        public void Execute(int i)
        {
            int2 vectorIndex = MapTools.IndexToVector(i, target.Resolution);
            float finalCountour = calculateDataRedMatrix(vectorIndex) + calculateDataBlueMatrix(vectorIndex);
            finalCountour = (finalCountour+2f)/4f;
            globalMap[arrayLenght * target.Index + i] = saturate(finalCountour);//(clamp(finalCountour,-totalAmount*2,totalAmount*2)+totalAmount*2)/(totalAmount*4f);
        }

        float calculateDataRedMatrix(int2 index){
            float result = 0;
            for(int dx = -EffectSize; dx <= EffectSize; dx++){
                int sng = (int)sign(dx);
                float currentNormal = getValueAtIndex(index.x+dx, index.y, 0)*sign(dx);
                result += currentNormal;
                if(dx == -EffectSize || dx == EffectSize){
                    float next = getValueAtIndex(index.x+dx+sng, index.y, 0)*sng;
                    result += next*ExtraStrength;
                }
            }
            result /= EffectSize+ExtraStrength;

            return result;
        }
        float calculateDataBlueMatrix(int2 index){          
            float result = 0;
            for(int dy = -EffectSize; dy <= EffectSize; dy++){
                int sng = (int)sign(dy);
                float currentNormal = getValueAtIndex(index.x, index.y+dy, 1)*sign(dy);
                result += currentNormal;

                if(dy == -EffectSize || dy == EffectSize){
                    float next = getValueAtIndex(index.x, index.y+dy+sng, 1)*sng;
                    result += next*ExtraStrength;
                }
                
            }
            result /= EffectSize+ExtraStrength;

            return result;
        }
        
        
        float getValueAtIndex(int x, int y, int baseIndex){
            int index = MapTools.GetFlatIndex(int2(x, y), target.Resolution, channelIndex.Resolution);
            return globalMap[arrayLenght * (channelIndex.Index+baseIndex) + index];
        }


        public static JobHandle ScheduleParallel(NativeArray<float> globalMap,      
            IndexAndResolution target, IndexAndResolution channelIndex,
            int EffectSize, float ExtraStrength,
            int arrayLenght, JobHandle dependency) => new GetCavityJob()
        {
            globalMap = globalMap,
            target = target,
            channelIndex = channelIndex,
            EffectSize = EffectSize,
            ExtraStrength = ExtraStrength,
            arrayLenght = arrayLenght,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}