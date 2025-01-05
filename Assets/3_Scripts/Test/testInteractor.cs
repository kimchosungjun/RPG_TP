using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInteractor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.AddInteractable(other.GetComponent<Interactable>());
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.RemoveInteractable(other.GetComponent<Interactable>());
        }
    }
}
