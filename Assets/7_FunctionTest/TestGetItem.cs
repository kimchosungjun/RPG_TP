using ItemEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetItem : MonoBehaviour
{
    [SerializeField] int etcID = 1;
    [SerializeField] int consumeID = 101;
    [SerializeField] int weaponID = 201;
    [SerializeField] int cnt;

    private void Update()
    {
        InputCheatKey();
#if UNITY_EDITOR
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetEtc();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetConsume();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetWeapon();
        }
    }

    public void GetEtc()
    {
        EtcData data = new EtcData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetEtcTableData(etcID), cnt);
        SharedMgr.InventoryMgr.AddItem(data);
    }

    public void GetConsume()
    {
        ConsumeData data = new ConsumeData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetConsumeTableData(consumeID), cnt);
        SharedMgr.InventoryMgr.AddItem(data);
    }

    public void GetWeapon()
    {
        WeaponData data = new WeaponData();
        data.SetData(SharedMgr.TableMgr.GetItem.GetWeaponTableData(weaponID));
        SharedMgr.InventoryMgr.AddItem(data);
    }
}
