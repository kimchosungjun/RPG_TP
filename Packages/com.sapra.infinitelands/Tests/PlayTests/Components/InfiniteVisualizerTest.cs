using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace sapra.InfiniteLands.Tests
{
    public class InfiniteVisualizerTest
    {   
        FloatingOrigin CreateOrigin(){
            var origin = new GameObject();
            return origin.AddComponent<FloatingOrigin>();
        }

        [UnityTest]
        public IEnumerator ISpawnComponent()
        {
        
            var origin = new GameObject();
            origin.AddComponent<InfiniteChunkVisualizer>();
            yield return new WaitForEndOfFrame();


            Object.Destroy(origin.gameObject);
        }


    }
}