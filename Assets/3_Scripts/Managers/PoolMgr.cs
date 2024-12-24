using System.Collections.Generic;
using UnityEngine;
using PoolEnums;

public class PoolMgr : MonoBehaviour
{
    // Pool Parent Group
    Dictionary<OBJECTS, Transform> poolParentGroup = new Dictionary<OBJECTS, Transform>();
    // Object Pool Group
    Dictionary<OBJECTS, List<Transform>> objectPoolGroup = new Dictionary<OBJECTS, List<Transform>>();    
    // Pool Original Group
    Dictionary<OBJECTS, Transform> originalGroup = new Dictionary<OBJECTS, Transform>();
    
    private void Awake()
    {
        SharedMgr.PoolMgr = this;
    }

    public Transform GetPool(OBJECTS _poolObject)
    {
        Transform result = null; 
        if (poolParentGroup.ContainsKey(_poolObject) == false)
        {
            string parentName = Enums.GetEnumString<OBJECTS>(_poolObject) + "Parent";

            GameObject go = new GameObject(parentName);
            go.transform.position = Vector3.zero;
            go.transform.SetParent(this.transform, true); // Second Parameter = true : maintain own world position
            poolParentGroup.Add(_poolObject, go.transform);  

            result = SharedMgr.ResourceMgr.LoadResource<Transform>("Pools/"+Enums.GetEnumString<OBJECTS>(_poolObject));
            originalGroup.Add(_poolObject, result);

            if(result == null) { Debug.LogError("없음"); return null; }
            result = Instantiate(result.gameObject).transform;

            objectPoolGroup[_poolObject] = new List<Transform>();
            objectPoolGroup[_poolObject].Add(result);
        }
        else
        {
             result = GetObjectInPool(_poolObject);
        }
        return result;
    }

    Transform GetObjectInPool(OBJECTS _poolObject)
    {
        List<Transform> list = objectPoolGroup[_poolObject];   
        int cnt = list.Count;
        for(int i = 0; i < cnt; i++)
        {
            if (list[i].gameObject.activeSelf == true)
                return list[i];
        }

        GameObject result = Instantiate(originalGroup[_poolObject].gameObject);
        list.Add(result.transform);   
        return result.transform;
    }
}
