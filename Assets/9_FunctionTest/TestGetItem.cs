using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetItem : MonoBehaviour
{
    [SerializeField] int etcId;
    [SerializeField] int cnt;


    private void OnGUI()
    {
        if(GUI.Button (new Rect(300,300,50,50), "기타 아이템 습득"))
        {
            GetEtc();
        }
        //if (GUI.Button(new Rect(400, 300, 50, 50), "소비 아이템 습득"))
        //{

        //}
        //if (GUI.Button(new Rect(500, 300, 50, 50), "무기 아이템 습득"))
        //{

        //}
    }

    public void GetEtc()
    {
        EtcData data = new EtcData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetEtcTableData(etcId), cnt);
        SharedMgr.InventoryMgr.AddItem(data);
    }
}
