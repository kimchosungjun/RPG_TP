using UnityEngine;
using UnityEngine.Pool;

namespace sapra.InfiniteLands{
    public class VegetationInstancePool 
    {
        private ObjectPool<GameObject> Pool;
        private GameObject gameObject;
        private Transform InstancesParent;
        public VegetationInstancePool(GameObject gameObject, Transform parent){
            Pool = new ObjectPool<GameObject>(GetInstance, actionOnDestroy: AdaptiveDestroy);
            this.gameObject = gameObject;

            GameObject trans = new GameObject(this.gameObject.name);
            InstancesParent = trans.transform;
            InstancesParent.SetParent(parent);
        }
        public void Dispose(){
            if(Pool != null)
                Pool.Dispose();
            
            if(InstancesParent != null){
                AdaptiveDestroy(InstancesParent.gameObject);
            }
        }

        public GameObject CreateInstance(InstanceData data)
        {
            GameObject AvailableCollider;
            if (data.GetValidity() <= 0)
                return null;
        
            AvailableCollider = Pool.Get();
            AvailableCollider.SetActive(true);

            AvailableCollider.transform.position = data.GetPosition();
            AvailableCollider.transform.rotation = data.GetRotation();
            AvailableCollider.transform.localScale = data.GetScale();

            return AvailableCollider;
        }

        public void Return(GameObject instance){
            if(instance != null){
                instance.SetActive(false);
                Pool.Release(instance);
            }
        }

        private GameObject GetInstance(){
            GameObject BJ = Object.Instantiate(gameObject, InstancesParent);
            BJ.AddComponent<FloatingPoint>();
            return BJ;
        }

        private void AdaptiveDestroy(UnityEngine.Object obj) {
            if (Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
        }
    }
}