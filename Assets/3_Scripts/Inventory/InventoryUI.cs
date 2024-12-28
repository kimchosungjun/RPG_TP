using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemEnums;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryObject;
    [SerializeField] InventoryListUI inventoryListUI;
    [SerializeField] InventoryInfoUI inventoryInfoUI;
    [SerializeField] ConsumeUseUI consumeUseUI;
    [SerializeField] SideBarSlot[] sideSlots; // 아틀라스로 이미지 설정
    ITEMTYPE currentShowType = ITEMTYPE.ITEM_ETC;
    public ITEMTYPE GetCurrentType { get { return currentShowType; } }
    int inventoryCurrentIndex = 0;

    public void Init()
    {
        
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
        currentShowType = _itemType;
        inventoryListUI.ChangeItemType(currentShowType);
    }

    #region Info UI
    public void ShowItemInfo(EtcData _itemData)
    {
        //inventoryInfoUI
    }

    public void ShowItemInfo(ConsumeData _itemData)
    {

    }
    public void ShowItemInfo(WeaponData _itemData)
    {

    }
    #endregion

    #region Use UI

    public void DecideUse(ConsumeData _data) { consumeUseUI.Active(_data); }
    #endregion

    public void AddItem() { }
    public void RemoveItem() { }
    
    
}


