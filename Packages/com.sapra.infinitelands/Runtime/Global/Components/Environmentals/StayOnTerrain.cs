using UnityEngine;

namespace sapra.InfiniteLands{
    public class StayOnTerrain : MonoBehaviour
    {
        public PointStore store;
        public float verticalOffset;
        public float treshold = 0;

        // Start is called before the first frame update
        void Start()
        {
            store = FindAnyObjectByType<PointStore>();
            store.onProcessDone += OnChunkChanges;
        }

        void OnChunkChanges(CoordinateDataHolder data){
            Vector3 ground = store.GetCoordinateDataAtPosition(transform.position,out bool found, true, false).position;
            if(!found)
                return;
            Vector3 current = store.ToLocalSpacePoint(transform.position);
            var dif = current-ground;
            if(dif.y < treshold)
                transform.position = store.ToWorldSpacePoint(ground+Vector3.up*verticalOffset); 
        }

        void OnDisable()
        {
            store.onProcessDone -= OnChunkChanges;
        }
    }
}