using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.AddInteractable(other.GetComponent<Interactable>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.RemoveInteractable(other.GetComponent<Interactable>());
        }
    }
}
