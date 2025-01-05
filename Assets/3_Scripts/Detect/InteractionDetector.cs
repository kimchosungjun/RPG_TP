using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    int detectLayer = (int)UtilEnums.LAYERS.INTERACTOR;
    [SerializeField] float detectorSize;

    public void Awake()
    {
        SphereCollider coll = GetComponent<SphereCollider>();
        if (coll != null)
        {
            coll.radius = detectorSize; 
            if(coll.isTrigger == false)
                coll.isTrigger = true;  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == detectLayer)
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == detectLayer)
        {

        }
    }
}
