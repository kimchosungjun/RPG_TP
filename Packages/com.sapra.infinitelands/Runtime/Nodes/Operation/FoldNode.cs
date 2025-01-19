using UnityEngine;
using Unity.Jobs;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Fold", type = "Operation", docs = "https://ensapra.com/packages/infinite_lands/nodes/fold.html")]
    public class FoldNode : HeightNodeBase
    {
        public enum Refe{Bottom, Top}
        [Range(0,1)] public float FoldingLine = 0.5f;
        public Refe RelativeTo;

        protected override Vector2 GetMinMaxValue()
        {
            Vector2 newMinMax;
            Vector2 currentMinMax = Input.minMaxValue;

            float amountOfValue = .5f-Mathf.Abs(FoldingLine-.5f);

            float displacement = currentMinMax.y-currentMinMax.x;
            switch(RelativeTo){
                case Refe.Top:
                {
                    newMinMax.x = currentMinMax.x+displacement*amountOfValue;
                    newMinMax.y = currentMinMax.y;
                    break;
                }
                default:
                {
                    newMinMax.x = currentMinMax.x;
                    newMinMax.y = currentMinMax.y-displacement*amountOfValue;
                    break;
                }
            }
            return newMinMax;
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {           
            HeightData previousJob = Input.RequestHeight(settings);
            switch(RelativeTo){
                case Refe.Top:{
                    return FoldJobTop.ScheduleParallel(settings.globalMap,previousJob.indexData, target,
                        FoldingLine,Input.minMaxValue,
                        settings.pointsLength, previousJob.jobHandle);
                }
                default:{
                    return FoldJobBottom.ScheduleParallel(settings.globalMap,previousJob.indexData, target,
                        FoldingLine, Input.minMaxValue,
                        settings.pointsLength, previousJob.jobHandle);
                }
            }
        }
    }
}