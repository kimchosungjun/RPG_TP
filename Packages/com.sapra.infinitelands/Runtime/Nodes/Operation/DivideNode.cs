using Unity.Jobs;
using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Divide", type = "Operation", docs = "https://ensapra.com/packages/infinite_lands/nodes/divide.html")]
    public class DivideNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return Dividend.minMaxValue;
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Dividend;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Divisor;

        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Dividend,Divisor};

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int res0 = Dividend.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            int res1 = Divisor.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            return Math.Max(res0, res1);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJobHeightDividend = Dividend.RequestHeight(settings);
            HeightData previousJobHeightDivisor = Divisor.RequestHeight(settings);

            return DivideJob.ScheduleParallel(settings.globalMap,
                previousJobHeightDividend.indexData, previousJobHeightDivisor.indexData, target,
                settings.pointsLength, 
                JobHandle.CombineDependencies(previousJobHeightDividend.jobHandle, previousJobHeightDivisor.jobHandle));
        }
    }
}