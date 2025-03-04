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
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GetEtc();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GetConsume();
        }
        if (Input.GetKeyDown(KeyCode.F3))
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
