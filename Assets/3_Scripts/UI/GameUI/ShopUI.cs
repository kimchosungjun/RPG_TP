using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject shopWindow;
    [SerializeField] ShopButton[] buttons;
    ItemData curItemData = null;
    //int itemCnt = 1;
    public ItemData CurItemData
    {
        get { return curItemData; }
        set { curItemData = value; /*itemCnt = 1; */}
    }

    public void SetShopItemList(List<ItemData> _datas)
    {
        int listCnt = _datas.Count;
        for(int i=0; i<listCnt; i++)
        {
            buttons[i].SetData(_datas[i]);
        }
        int buttonCnt = buttons.Length;
        for(int i=listCnt; i<buttonCnt; i++)
        {
            buttons[i].SetData(null);
        }

        shopWindow.SetActive(true);
        //SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.SHOP;
    }


    public void BuyItem()
    {
        if(curItemData == null)
            return;
        if (SharedMgr.InventoryMgr.CanUseGold(curItemData.itemCnt * 100) == false)
            return;

        SharedMgr.InventoryMgr.RemoveGold(curItemData.itemCnt * 100);
        //SharedMgr.InventoryMgr.AddItem()
    }

    public void CloseShop()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        shopWindow.SetActive(false);
        SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PopUIPopup();
    }
}
