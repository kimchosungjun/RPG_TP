using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgrade : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.GetPlayerStatControl.GetExp(10000);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SharedMgr.InventoryMgr.AddGold(10000);
        }
    }
}
