using UnityEngine;

namespace sapra.InfiniteLands{
    public class AlignWithTerrain : MonoBehaviour
    {
        public enum GroundAlignement{None, Ground, Terrain}
        public GroundAlignement AlignementMode = GroundAlignement.None;
        public PointStore store;
        public float SmoothTransitionToPosition = 0;

        private Vector3 targetPosition;
        private Vector3 targetNormal;

        private Vector3 refVelocity;

        private FloatingPoint pnt;


        // Start is called before the first frame update
        void Start()
        {
            pnt = GetComponent<FloatingPoint>();
            store = FindFirstObjectByType<PointStore>();
            store.onProcessDone += OnChunkChanges;
            targetPosition = transform.position;
            if(pnt != null)
                pnt.OnOffsetAdded += ApplyOffset;
        }

        void OnChunkChanges(CoordinateDataHolder _){
            ChunkData data = store.GetChunkDataAtPosition(transform.position);
            if(data != null){
                CoordinateData current = store.GetCoordinateDataAtPosition(transform.position,out bool found, true);

                Vector3 position = current.position;
                Vector3 normal = current.normal;
                if(found && position != targetPosition){
                    targetPosition = position;
                    targetNormal = normal;
                }
                if(data.terrainConfig.ID.z <= 0)
                    store.onProcessDone -= OnChunkChanges;
            }
        }

        void ApplyOffset(Vector3 offset){
            targetPosition += offset;
        }

        void OnDisable()
        {
            store.onProcessDone -= OnChunkChanges;
            if(pnt != null)
                pnt.OnOffsetAdded -= ApplyOffset;
        }

        void Update()
        {
            if(transform.position != targetPosition){
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, SmoothTransitionToPosition);
                if(Vector3.SqrMagnitude(targetPosition-transform.position) < 0.1f){
                    transform.position = targetPosition;
                    UpdateRotation();
                }
            }
        }

        private void UpdateRotation(){
            switch(AlignementMode){
                case GroundAlignement.Ground:
                    transform.rotation = Quaternion.FromToRotation(transform.up, targetNormal)*transform.rotation;
                    break;
                case GroundAlignement.Terrain:
                    transform.rotation = Quaternion.FromToRotation(transform.up, store.ToWorldSpaceVector(Vector3.up))*transform.rotation;
                    break;

            }
        }
    }
}