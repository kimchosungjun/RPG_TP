using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sapra.InfiniteLands{
    [System.Serializable]
    public struct Vector3Double{
        public double x;
        public double y;
        public double z;
        public Vector3Double(double x, double y, double z){
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector3Double operator +(Vector3Double a, Vector3 b){
            return new Vector3Double(a.x+b.x, a.y+b.y, a.z+b.z);
        }
        public static Vector3Double operator +(Vector3Double a, Vector3Double b){
            return new Vector3Double(a.x+b.x, a.y+b.y, a.z+b.z);
        }
        public static Vector3Double operator -(Vector3Double a, Vector3 b){
            return new Vector3Double(a.x-b.x, a.y-b.y, a.z-b.z);
        }

        public static Vector3Double operator -(Vector3Double a, Vector3Double b){
            return new Vector3Double(a.x-b.x, a.y-b.y, a.z-b.z);
        }

        public static implicit operator Vector3(Vector3Double a)
        {
            return new Vector3((float)a.x, (float)a.y, (float)a.z);
        }
    }   
    public class FloatingOrigin : MonoBehaviour
    {
        
        [SerializeField] private Transform originReference;
        [SerializeField] private float MaxDistance = 1000;
        [SerializeField] private Vector3Double CurrentOrigin;

        public Action<Vector3Double, Vector3Double> OnOriginMove;

        [SerializeField][Range(0,1)] private float ClosenessToEdge;
        [SerializeField] private bool DrawMovementSpace;

        // Start is called before the first frame update
        void Start()
        {
            transform.position = Vector3.zero;
            if(originReference == null)
                Debug.LogErrorFormat("Missing {0}, no origin shifts will happen", nameof(originReference));
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(originReference == null)
                return;
            float distance = Vector3.Distance(originReference.position, Vector3.zero);
            if(distance > MaxDistance){
                Vector3Double old = CurrentOrigin;
                CurrentOrigin += originReference.position;
                OnOriginMove?.Invoke(CurrentOrigin, old);
            }   

            ClosenessToEdge = distance/MaxDistance;
        }

        public Vector3Double GetCurrentOrigin() => CurrentOrigin;
        public void SetOriginReference(Transform ogReference) => originReference = ogReference;
        public float SetMaxDistance(float distance) => MaxDistance = distance;
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(DrawMovementSpace){
                Handles.DrawWireDisc(Vector3.zero,Vector3.up, MaxDistance);
                Gizmos.color = Color.red;
                if(originReference != null)
                    Gizmos.DrawCube(originReference.position, Vector3.one*10);

            }
        }
        #endif
    }
}