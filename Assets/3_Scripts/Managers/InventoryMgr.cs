using UnityEngine;
using System;
using ItemEnums;
using System.Collections.Generic;
using SaveDataGroup;
public class InventoryMgr  
{
    /**********************************************/
    /************ 인벤토리 변수 ****************/
    /**********************************************/
    
    #region Hold Item : Private
    int gold = 0;

    Dictionary<int, List<EtcData>> etcGroup = new Dictionary<int, List<EtcData>>();
    Dictionary<int, List<ConsumeData>> consumeGroup = new Dictionary<int, List<ConsumeData>>();
    
    int maxGold = 9999999;
    int inventoryMaxCnt = 25;
    List<EtcData> etcDatas = new List<EtcData>();
    List<ConsumeData> consumeDatas = new List<ConsumeData>();   
    List<WeaponData> weaponDatas = new List<WeaponData>();

    Dictionary<int,WeaponData> holdWeaponGroup = new Dictionary<int, WeaponData>();
    Dictionary<WEAPONTYPE, List<WeaponData>> weaponSortGroup = new Dictionary<WEAPONTYPE, List<WeaponData>>();
    public Dictionary<WEAPONTYPE, List<WeaponData>> GetTestData { get { return weaponSortGroup; } }
    #endregion

    #region Hold Item : Property
    public int Gold { get { return gold; } set { gold = value; } }
    public List<EtcData> GetEtcInventory()  { return etcDatas; }
    public List<ConsumeData> GetConsumeInventory() { return consumeDatas; }
    public List<WeaponData> GetWeaponInventory() { return weaponDatas; }
    public List<WeaponData> GetHoldWeaponInventory() 
    {
        List<int>keys = new List<int>(holdWeaponGroup.Keys);
        int cnt = keys.Count;
        if(cnt ==0) return null;
        List<WeaponData> holdList = new List<WeaponData>();
        for(int i=0; i < cnt; i++)
        {
            holdList.Add(holdWeaponGroup[keys[i]]);
        }
        return holdList; 
    }

    Queue<ItemData> getItemQueue = new Queue<ItemData>();

    public ItemData GetItemData(int _itemID)
    {
        int etcCnt = etcDatas.Count;
        for(int i=0; i<etcCnt; i++)
        {
            if (etcDatas[i].itemID == _itemID)
                return etcDatas[i]; 
        }

        int consumeCnt = etcDatas.Count;
        for (int i = 0; i < consumeCnt; i++)
        {
            if (etcDatas[i].itemID == _itemID)
                return etcDatas[i];
        }
        return null;
    }
    #endregion

    /**********************************************/
    /************ 인벤토리 관리 ****************/
    /**********************************************/

    #region Check Can Add Item : Check Inventory Size
    public bool CanUseGold(int _gold) { return (gold - _gold >= 0); }

    public bool CanAddItem(EtcData _etcData)
    {
        if (etcGroup.ContainsKey(_etcData.itemID))
        {
            List<EtcData> datas = etcGroup[_etcData.itemID];
            int cnt = etcGroup[_etcData.itemID].Count;
            int itemCnt = _etcData.itemCnt;

            for (int i = 0; i < cnt; i++)
            {
                if (datas[i].itemCnt == _etcData.GetMaxCnt)
                    continue;
                if (datas[i].itemCnt + itemCnt <= _etcData.GetMaxCnt)
                    return true;
                else
                    itemCnt = _etcData.GetMaxCnt - datas[i].itemCnt;
            }

            if (itemCnt != 0)
            {
                if (etcDatas.Count < inventoryMaxCnt)
                    return true;
               return false;
            }
            return true;
        }
        else
        {
            if (etcDatas.Count < inventoryMaxCnt)
                return true;
            return false;
        }
    }

    public bool CanAddItem(ConsumeData _consumeData)
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
                    return true;
                else
                    itemCnt = _consumeData.GetMaxCnt - datas[i].itemCnt;
            }

            if (itemCnt != 0)
            {
                if (consumeDatas.Count < inventoryMaxCnt)
                    return true;
                return false;
            }
            return true;
        }
        else
        {
            if (consumeDatas.Count < inventoryMaxCnt)
                return true;
            return false;
        }
    }

    public bool CanAddItem(WeaponData _weaponData)
    {
        int cnt = weaponDatas.Count;
        if (cnt >= inventoryMaxCnt)
            return false;
        return true;
    }
    #endregion

    #region Add Item
    public void AddGold(int _gold)
    {
        gold = (gold + _gold) > maxGold ? maxGold : gold + _gold;
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.GetGoldUI.UpdateGold();
    }

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
                    ShowGetSlot(_etcData);
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
                    ShowGetSlot(_etcData);
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
                ShowGetSlot(_etcData);
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
                    ShowGetSlot(_consumeData);
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
                    ShowGetSlot(_consumeData);
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
                ShowGetSlot(_consumeData);
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
        WEAPONTYPE type = _weaponData.WeaponType;

        weaponDatas.Add(_weaponData);
        if (weaponSortGroup.ContainsKey(type) == false)
            weaponSortGroup[type] = new List<WeaponData>();
        List<WeaponData> list = weaponSortGroup[type];
        list.Add(_weaponData);
        weaponSortGroup[type] = list;
        if (SharedMgr.UIMgr.GameUICtrl.CanAccessUI() == false) return;
        ShowGetSlot(_weaponData);
    }

    #endregion

    #region Remove Item
    public void RemoveGold(int _gold) 
    {
        if (CanUseGold(_gold) == false) return;
        gold -= _gold; 
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.GetGoldUI.UpdateGold();
    }

    public void RemoveItem(EtcData _etcData)
    {
        if (etcGroup.ContainsKey(_etcData.itemID) == false)
            return;
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.CurrentItemData = null;
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
                SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
                break;
            }
        }
    }

    public void RemoveItem(ConsumeData _consumeData)
    {
        if (consumeGroup.ContainsKey(_consumeData.itemID) == false)
            return;

        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.CurrentItemData = null;
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
                SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
                break;
            }
        }
    }

    public void RemoveItem(WeaponData _weaponData)
    {
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.CurrentItemData = null;
        int cnt= weaponDatas.Count;
        for(int i=0; i<cnt; i++)
        {
            if (weaponDatas[i] == _weaponData)
            {
                weaponDatas.RemoveAt(i);
                SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
                break;
            }
        }

        if (weaponSortGroup.ContainsKey(_weaponData.WeaponType) == false)
            return;
        List<WeaponData> list = weaponSortGroup[_weaponData.WeaponType];
        int listCnt= list.Count;    
        for(int i=0; i<listCnt; i++)
        {
            if (list[i] == _weaponData)
            {
                weaponSortGroup[_weaponData.WeaponType].RemoveAt(i);    
                return;
            }
        }
    }

    #endregion

    #region Link Show Get Item UI
    public void ShowGetSlot(ItemData _itemData)
    {
        getItemQueue.Enqueue(_itemData);
        ShowGetItemSlot slot = SharedMgr.PoolMgr.GetItemSlot();
        slot?.ShowSlot(getItemQueue.Dequeue());
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
    }

    public void ShowNextGetItemSlot()
    {
        if (getItemQueue.Count == 0)
            return;
        SharedMgr.PoolMgr.GetItemSlot()?.ShowSlot(getItemQueue.Dequeue());
    }
    #endregion

    #region Hold Weapon 
    public void ChangeHoldWeapon(WeaponData _legacyWeapon ,WeaponData _newWeapon, int _characterID)
    {
        if (_legacyWeapon!=null && holdWeaponGroup.ContainsKey(_legacyWeapon.uniqueID))
        {
            holdWeaponGroup[_legacyWeapon.uniqueID].TakeOff();
            holdWeaponGroup.Remove(_legacyWeapon.uniqueID);
        }
        _newWeapon.holdPlayerID = _characterID;
        _newWeapon.Use(_characterID);
        holdWeaponGroup.Add(_newWeapon.uniqueID, _newWeapon);
    }

    public List<WeaponData> GetSortWeaponGroup(WEAPONTYPE _weaponType)
    {
        if (weaponSortGroup.ContainsKey(_weaponType)==false) return null;

        int cnt = weaponSortGroup[_weaponType].Count;
        if (cnt == 0) return null;
        return weaponSortGroup[_weaponType];
    }

    public WeaponData GetHoldWeaponData(int _uniqueID)
    {
        if(holdWeaponGroup.ContainsKey(_uniqueID))
            return holdWeaponGroup[_uniqueID];
        return null;
    }

    #endregion

    #region Manage Data

    public void Init()
    {
        SharedMgr.InventoryMgr = this;
    }

    public void ClearAllData()
    {
        if (etcGroup.Count > 0)
            etcGroup.Clear();
        if (consumeGroup.Count > 0)
            consumeGroup.Clear();
        if (etcDatas.Count > 0)
            etcDatas.Clear();
        if (consumeDatas.Count > 0)
            consumeDatas.Clear();
        if (weaponDatas.Count > 0)
            weaponSortGroup.Clear();
        if (holdWeaponGroup.Count > 0)
            holdWeaponGroup.Clear();
        if (weaponSortGroup.Count > 0)
            weaponSortGroup.Clear();
    }

    public void LoadInventory(InventorySaveDataGroup _saveDatas)
    {
        ClearAllData();
        etcDatas = _saveDatas.EtcSet;
        consumeDatas = _saveDatas.ConsumeSet;
        weaponDatas = _saveDatas.WeaponSet;
        gold = _saveDatas.Gold;

        ItemTable table = SharedMgr.TableMgr.GetItem;
        // Etc
        int etcCnt = etcDatas.Count;
        for(int i=0; i<etcCnt; i++)
        {
            etcDatas[i].LoadData(table.GetEtcTableData(etcDatas[i].itemID));
            if (etcGroup.ContainsKey(etcDatas[i].itemID))
                etcGroup[etcDatas[i].itemID].Add(etcDatas[i]);
            else
            {
                List<EtcData> overlapDatas = new List<EtcData> ();
                overlapDatas.Add(etcDatas[i]);
                etcGroup.Add(etcDatas[i].itemID, overlapDatas);
            }
        }

        // Consume
        int consumeCnt = consumeDatas.Count;
        for (int i = 0; i < consumeCnt; i++)
        {
            consumeDatas[i].LoadData(table.GetConsumeTableData(consumeDatas[i].itemID));
            if (consumeGroup.ContainsKey(consumeDatas[i].itemID))
                consumeGroup[consumeDatas[i].itemID].Add(consumeDatas[i]);
            else
            {
                List<ConsumeData> overlapDatas = new List<ConsumeData>();
                overlapDatas.Add(consumeDatas[i]);
                consumeGroup.Add(consumeDatas[i].itemID, overlapDatas);
            }
        }

        // Weapon
        int weaponCnt = weaponDatas.Count;
        for (int i = 0; i < weaponCnt; i++)
        {
            weaponDatas[i].LoadData(table.GetWeaponTableData(weaponDatas[i].itemID));
            // Hold Weapon
            if (weaponDatas[i].IsHoldWeapon)
                holdWeaponGroup.Add(weaponDatas[i].uniqueID, weaponDatas[i]);
            // Weapon Sort
            WEAPONTYPE type = weaponDatas[i].WeaponType;
            if (weaponSortGroup.ContainsKey(type))
            {
                weaponSortGroup[type].Add(_saveDatas.WeaponSet[i]);
            }
            else
            {
                List<WeaponData> datas = new List<WeaponData>();
                datas.Add(weaponDatas[i]);
                weaponSortGroup.Add(type, datas);
            }
        }

        gold = _saveDatas.Gold;
    }
    #endregion
}
