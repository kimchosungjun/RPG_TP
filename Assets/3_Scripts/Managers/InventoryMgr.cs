using UnityEngine;
using System;
using ItemEnums;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class InventoryMgr  
{
    // To Do ~~~ Gold

    #region List
    Dictionary<int, List<EtcData>> etcGroup = new Dictionary<int, List<EtcData>>();
    Dictionary<int, List<ConsumeData>> consumeGroup = new Dictionary<int, List<ConsumeData>>();
    
    int inventoryMaxCnt = 25;
    List<EtcData> etcDatas = new List<EtcData>();
    List<ConsumeData> consumeDatas = new List<ConsumeData>();   
    List<WeaponData> weaponDatas = new List<WeaponData>();
    #endregion

    #region Get List
    public List<EtcData> GetEtcInventory()  {  return etcDatas; }
    public List<ConsumeData> GetConsumeInventory() { return consumeDatas; }
    public List<WeaponData> GetWeaponInventory() { return weaponDatas; }
    #endregion


    #region Add Item

    public void AddItem(EtcData _etcData)
    {
        if (etcGroup.ContainsKey(_etcData.itemID))
        {
            List<EtcData> datas = etcGroup[_etcData.itemID];
            int cnt = etcGroup[_etcData.itemID].Count;
            int itemCnt = _etcData.itemCnt;

            for(int i=0; i<cnt; i++)
            {
                if (datas[i].itemCnt == _etcData.GetMaxCnt)
                    continue;
                if (datas[i].itemCnt + itemCnt <= _etcData.GetMaxCnt)
                {
                    datas[i].itemCnt += itemCnt;
                    return;
                }
                else
                {
                    itemCnt = _etcData.GetMaxCnt - datas[i].itemCnt;
                    datas[i].itemCnt = _etcData.GetMaxCnt;
                }
            }

            if(itemCnt !=0)
            {
                if (etcDatas.Count < inventoryMaxCnt)
                {
                    _etcData.itemCnt = itemCnt;
                    etcDatas.Add(_etcData);
                    etcGroup[_etcData.itemID].Add(_etcData);
                }
                else
                {
                    // To Do Link UI
                    Debug.Log("Full Inventory");
                }
            }
        }
        else
        {
            if (etcDatas.Count < inventoryMaxCnt)
            {
                etcGroup[_etcData.itemID]= new List<EtcData> ();
                etcGroup[_etcData.itemID].Add(_etcData);
                etcDatas.Add(_etcData);
            }
            else
            {
                // To Do Link UI
                Debug.Log("Full Inventory");
            }
        }
    }

    public void AddItem(ConsumeData _consumeData)
    {
        if (consumeGroup.ContainsKey(_consumeData.itemID))
        {
            List<ConsumeData> datas = consumeGroup[_consumeData.itemID];
            int cnt = consumeGroup[_consumeData.itemID].Count;
            int itemCnt = _consumeData.itemCnt;

            for (int i = 0; i < cnt; i++)
            {
                if (datas[i].itemCnt == _consumeData.GetMaxCnt)
                    continue;
                if (datas[i].itemCnt + itemCnt <= _consumeData.GetMaxCnt)
                {
                    datas[i].itemCnt += itemCnt;
                    return;
                }
                else
                {
                    itemCnt = _consumeData.GetMaxCnt - datas[i].itemCnt;
                    datas[i].itemCnt = _consumeData.GetMaxCnt;
                }
            }

            if (itemCnt != 0)
            {
                if (consumeDatas.Count < inventoryMaxCnt)
                {
                    _consumeData.itemCnt = itemCnt;
                    consumeDatas.Add(_consumeData);
                    consumeGroup[_consumeData.itemID].Add(_consumeData);
                }
                else
                {
                    // To Do Link UI
                    Debug.Log("Full Inventory");
                }
            }
        }
        else
        {
            if (consumeDatas.Count < inventoryMaxCnt)
            {
                consumeGroup[_consumeData.itemID] = new List<ConsumeData>();
                consumeGroup[_consumeData.itemID].Add(_consumeData);
                consumeDatas.Add(_consumeData);
            }
            else
            {
                // To Do Link UI
                Debug.Log("Full Inventory");
            }
        }
    }

    public void AddItem(WeaponData _weaponData)
    {
        int cnt = weaponDatas.Count;
        if(cnt>=inventoryMaxCnt)
        {
            // To Do Link UI
            Debug.Log("Full Inventory");
            return;
        }    
        weaponDatas.Add(_weaponData);
    }

    #endregion

    #region Remove Item

    public void RemoveItem(EtcData _etcData)
    {
        if (etcGroup.ContainsKey(_etcData.itemID) == false)
            return;

        int groupCnt = etcGroup[_etcData.itemID].Count;
        for (int i = 0; i < groupCnt; i++)
        {
            if (etcGroup[_etcData.itemID][i] == _etcData)
            {
                etcGroup[_etcData.itemID].RemoveAt(i);
                break;
            }
        }

        int listCnt= etcDatas.Count;
        for(int i=0; i<listCnt; i++)
        {
            if (etcDatas[i] == _etcData)
            {
                etcDatas.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveItem(ConsumeData _consumeData)
    {
        if (consumeGroup.ContainsKey(_consumeData.itemID) == false)
            return;

        int groupCnt = consumeGroup[_consumeData.itemID].Count;
        for (int i = 0; i < groupCnt; i++)
        {
            if (consumeGroup[_consumeData.itemID][i] == _consumeData)
            {
                consumeGroup[_consumeData.itemID].RemoveAt(i);
                break;
            }
        }

        int listCnt = consumeDatas.Count;
        for (int i = 0; i < listCnt; i++)
        {
            if (consumeDatas[i] == _consumeData)
            {
                consumeDatas.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveItem(WeaponData _weaponData)
    {
        int cnt= weaponDatas.Count;
        for(int i=0; i<cnt; i++)
        {
            if (weaponDatas[i] == _weaponData)
            {
                weaponDatas.RemoveAt(i);
                break;
            }
        }
    }

    #endregion

    public void Init()
    {
        SharedMgr.InventoryMgr = this;
    }

    public void LoadInventory()
    {
        // Load Inventory 
        // 1) Check Login
        // 2) Load
        // * Call LoginScene 
    }
}
