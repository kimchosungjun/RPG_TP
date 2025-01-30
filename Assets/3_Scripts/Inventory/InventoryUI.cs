using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemEnums;

public class InventoryUI : MonoBehaviour
{
    #region UI
    [SerializeField, Header("UI Parent : Use for On/Off")] GameObject inventoryObject;
    [SerializeField] InventorySideBarUI sideUI;
    [SerializeField] InventoryListUI inventoryListUI;
    [SerializeField] InventoryInfoUI inventoryInfoUI;
    [SerializeField] ConsumeUseUI consumeUseUI;
    [SerializeField] InventoryDeleteUI deleteUI;
    [SerializeField] InventoryExitUI exitUI;
    [SerializeField] InventoryGoldUI goldUI;

    public InventoryDeleteUI GetDeleteUI { get {return deleteUI; } }
    public InventoryGoldUI GetGoldUI { get { return goldUI; } }
    #endregion

    #region Item Data & Type

    ITEMTYPE currentShowType = ITEMTYPE.ITEM_ETC;
    public ITEMTYPE GetCurrentType { get { return currentShowType; } }

    ItemData currentItemData = null;
    public ItemData CurrentItemData { get { return currentItemData; } set { currentItemData = value; } }
    #endregion

    int inventoryCurrentIndex = 0;

    public void Init()
    {
        sideUI.Init();
        inventoryListUI.Init();
        inventoryInfoUI.Init();
        consumeUseUI.Init();
        deleteUI.Init();
        exitUI.Init();
        goldUI.Init();
    }

    public void PressBackGround()
    {
        if (consumeUseUI.IsActive())
            return;

        if (deleteUI.IsActive())
        {
            deleteUI.InActive();
            return;
        }

        inventoryInfoUI.TurnOffCurrentInfo();
    }

    #region Input : I (Only Window)
    public void InputInventoryKey()
    {
        if (inventoryObject.activeSelf == true)
        {
            InActiveAllUI();
            SharedMgr.CursorMgr.SetCursorVisibleState(false);
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.NONE;
        }
        else
        {
            inventoryObject.SetActive(true);
            ChangeShowItemType(currentShowType);
            SharedMgr.CursorMgr.SetCursorVisibleState(true);
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.INVENTORY;
        }
    }
    #endregion

    #region Side Bar UI
    public void ChangeShowItemType(ITEMTYPE _itemType)
    {
        if(currentShowType != _itemType)
        {
            inventoryInfoUI.TurnOffCurrentInfo();
            currentShowType = _itemType;
        }
        inventoryListUI.ChangeItemType(currentShowType);
    }
    #endregion

    #region Info UI
    public void ShowItemInfo(EtcData _itemData)
    {
        inventoryInfoUI.ShowInfo(_itemData);
    }

    public void ShowItemInfo(ConsumeData _itemData)
    {
        inventoryInfoUI.ShowInfo(_itemData);
    }

    public void ShowItemInfo(WeaponData _itemData)
    {
        inventoryInfoUI.ShowInfo(_itemData);
    }
    #endregion

    #region Use UI
    public void DecideUse(ConsumeData _data) 
    {
        if (_data == null) return;
        consumeUseUI.Active(_data); 
    }
    #endregion

    #region Add/Remove Item
    public void AddItem() { }
    public void RemoveItem() { }
    #endregion

    #region Decide Active UI State
    public void PressExitButton()
    {
        if (deleteUI.IsActive())
        {
            deleteUI.InActive();
            return;
        }

        if(consumeUseUI.IsActive())
        {
            consumeUseUI.InActive();
            return;
        }
        SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.NONE;
        inventoryInfoUI.TurnOffCurrentInfo();
        inventoryObject.SetActive(false);
    }

    public void InActiveAllUI()
    {
        inventoryInfoUI.TurnOffCurrentInfo();
        deleteUI.InActive();
        consumeUseUI.InActive();
        inventoryObject.SetActive(false);
    }
    #endregion

    #region Update Inventory

    public void UpdateInventory()
    {
        inventoryListUI.ChangeItemType(currentShowType);
        inventoryInfoUI.UpdateInfoData();
    }

    #endregion
}


