using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class IndexManager
    {
        Dictionary<string, HeightGenerationResult> requests = new Dictionary<string, HeightGenerationResult>();
        public virtual void Reset()
        {
            IEnumerable<HeightGenerationResult> values = requests.Values;
            foreach(HeightGenerationResult indices in values){
                indices.Reset();
            }
        }

        public void ModifyCustomRequest(HeightGenerationResult request, InfiniteLandsNode requestor, string BranchGUID){
            string guid = requestor.guid + BranchGUID;
            if(requests.ContainsKey(guid)){
                requests[guid] = request;
            }
            else
                requests.TryAdd(guid, request);
        }

        public HeightGenerationResult GetIndices(InfiniteLandsNode requestor, string BranchGUID, out bool generated){
            string guid = requestor.guid + BranchGUID;
            if(!requests.TryGetValue(guid, out HeightGenerationResult request)){
                request = new HeightGenerationResult();            
                requests.Add(guid, request);
                generated = true;
            }
            else
                generated = false;
            return request;
        }
    }
}