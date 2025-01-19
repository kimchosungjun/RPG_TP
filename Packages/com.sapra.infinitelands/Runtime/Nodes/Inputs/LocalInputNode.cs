using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Local Input", type = "Input", docs ="https://ensapra.com/packages/infinite_lands/nodes/local.html")]
    public class LocalInputNode : HeightDataGenerator
    {
        [Input(typeof(IGive<HeightData>))] [HideInInspector] public LocalOutputNode output;

        protected override Vector2 GetMinMaxValue()
        {
            return output.inputMap.minMaxValue;
        }
        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{output};

/*         protected override bool PreInitilizeVerification()
        {
            if(output == null)
                Debug.LogWarningFormat("An output node must be selected in the drowpdown of {0}", this.name);
            return output != null && base.PreInitilizeVerification();
        } */

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int res = output.inputMap.PrepareNode(manager,ref currentCount, resolution, ratio, requestGuid);
            manager.ModifyCustomRequest(manager.GetIndices(output.inputMap,requestGuid, out bool generated), this, requestGuid);
            if(generated)
                Debug.LogErrorFormat("Missing {0}", guid);
            return res;
        }

        public override HeightData RequestHeight(GenerationSettings settings)
        {
            return output.inputMap.RequestHeight(settings);
        }

        public override HeightData RequestHeight(HeightGenerationResult ind, GenerationSettings settings)
        {
            return output.inputMap.RequestHeight(ind,settings);
        }
    }
    
}