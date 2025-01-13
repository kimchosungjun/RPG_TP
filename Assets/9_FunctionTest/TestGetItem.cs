using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetItem : MonoBehaviour
{
    [SerializeField] int etcId;
    [SerializeField] int cnt;
    [SerializeField] int weaponID;


    private void OnGUI()
    {
        if(GUI.Button (new Rect(300,300,50,50), "기타 아이템 습득"))
        {
            GetEtc();
        }
        if (GUI.Button(new Rect(400, 300, 50, 50), "소비 아이템 습득"))
        {
            GetConsume();
        }
        if (GUI.Button(new Rect(500, 300, 50, 50), "무기 아이템 습득"))
        {
            GetWeapon();
        }
        //if (GUI.Button(new Rect(100, 600, 50, 50), "카메라 효과"))
        //{
        //    PressZoom();
        //}

    }

    public void GetEtc()
    {
        EtcData data = new EtcData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetEtcTableData(etcId), cnt);
        SharedMgr.InventoryMgr.AddItem(data);
    }

    public void GetConsume()
    {
        ConsumeData data = new ConsumeData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetConsumeTableData(100));
        SharedMgr.InventoryMgr.AddItem(data);
    }

    public void GetWeapon()
    {
        WeaponData data = new WeaponData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetWeaponTableData(weaponID));
        SharedMgr.InventoryMgr.AddItem(data);
    }


    public void PressZoom()
    {
        float ScaleTime = 0.2f;
        float SlowTime = 3f;
        float SlowTimeConvertSlow = ScaleTime * SlowTime;
        SharedMgr.mainCam.ZoomEndStage(0, -1.5f, 2, SlowTime -1.5f, 1f, Vector3.zero);
    }
}
