using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealField : MonoBehaviour
{
    int playerLayer = 0;
    bool isInField = false;
    private void Start()
    {
        playerLayer = (int)UtilEnums.LAYERS.PLAYER;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer && isInField ==false)
        {
            isInField = true;
            SharedMgr.UIMgr.GameUICtrl.GetIndicatorUI.ActiveHealIndicator();
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.HealAllPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            isInField = false;
        }
    }
}
