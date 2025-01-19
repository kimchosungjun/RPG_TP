using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Local Output", type = "Output", docs = "https://ensapra.com/packages/infinite_lands/nodes/local.html")]
    public class LocalOutputNode : InfiniteLandsNode
    {
        [HideInInspector] public string OutputName = "Output";
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator inputMap;

        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{inputMap};
    }
}