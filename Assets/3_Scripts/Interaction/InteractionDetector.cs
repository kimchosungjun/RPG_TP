using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("Detect Interaction Distance")]
    [SerializeField] float detectRange;
    RaycastHit hit;

    public void FixedUpdate()
    {
        DetectInteractableObject();
    }

    public void DetectInteractableObject()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, detectRange, 1 << (int)UtilEnums.LAYERS.INTERACTOR);
        if (colls.Length == 0)
            return;


    }
}
