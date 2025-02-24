using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemEnums;

public class InventoryUI : UIBase
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
    [SerializeField] Animator anim;
    #endregion

    //int inventoryCurrentIndex = 0;

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

        isActiveState = false;
        inventoryInfoUI.TurnOffCurrentInfo();
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
    }

    #region Input : I (Only Window)
    public override void InputKey()
    {
        if (inventoryObject.activeSelf == true)
        {
            anim.Play("Idle");
            InActiveAllUI();
            SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PopUIPopup();
            isActiveState = false;
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
        }
        else
        {
            anim.Play("InvenOpen");
            inventoryObject.SetActive(true);
            ChangeShowItemType(currentShowType);
            SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PushUIPopup(this);
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true);
            isActiveState = true;
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
        SharedMgr.SoundMgr.PressButtonSFX();
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
        SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PopUIPopup();
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


