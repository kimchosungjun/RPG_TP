using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemEnums;

public class InventoryUI : MonoBehaviour
{
    [SerializeField, Header("UI Parent : Use for On/Off")] GameObject inventoryObject;
    [SerializeField] InventorySideBarUI sideUI;
    [SerializeField] InventoryListUI inventoryListUI;
    [SerializeField] InventoryInfoUI inventoryInfoUI;
    [SerializeField] ConsumeUseUI consumeUseUI;
   
    ITEMTYPE currentShowType = ITEMTYPE.ITEM_ETC;
    public ITEMTYPE GetCurrentType { get { return currentShowType; } }
    int inventoryCurrentIndex = 0;

    public void Init()
    {
        sideUI.Init();
        inventoryListUI.Init();
    }

    public void InputInventoryKey()
    {
        if (inventoryObject.activeSelf == true)
        {
            inventoryObject.SetActive(false);
        }
        else
        {
            inventoryObject.SetActive(true);
            ChangeShowItemType(currentShowType);
        }
    }

    public void ChangeShowItemType(ITEMTYPE _itemType)
    {
        if(currentShowType != _itemType) 
            inventoryInfoUI.TurnOffCurrentInfo();

        currentShowType = _itemType;
        inventoryListUI.ChangeItemType(currentShowType);

    }

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

    public void DecideUse(ConsumeData _data) { consumeUseUI.Active(_data); }
    #endregion

    public void AddItem() { }
    public void RemoveItem() { }
    
    
}


