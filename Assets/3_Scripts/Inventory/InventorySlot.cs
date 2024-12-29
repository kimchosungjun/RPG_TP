using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemEnums;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Text numberText;

    EtcData etcData = null;
    ConsumeData consumeData = null;
    WeaponData weaponData = null;

    // Empty Slot
    public void ChangeSlot()
    {
        itemImage.sprite = null;
        numberText.text = string.Empty;
    }

    public void ChangeSlot(EtcData _etcData)
    {
        itemImage.sprite = _etcData.itemIcon;
        numberText.text = _etcData.itemCnt+"개";
        etcData = _etcData;
    }

    public void ChangeSlot(ConsumeData _consumeData)
    {
        itemImage.sprite = _consumeData.itemIcon;
        numberText.text = _consumeData.itemCnt + "개";
        consumeData = _consumeData;
    }

    public void ChangeSlot(WeaponData _weaponData)
    {
        itemImage.sprite = _weaponData.itemIcon;
        numberText.text = "Lv."+_weaponData.weaponCurrentLevel;
        weaponData = _weaponData;   
    }

    public void PressSlot()
    {
        InventoryUI invenUI = SharedMgr.UIMgr.GameUICtrl.GetInventoyUI;
        switch (invenUI.GetCurrentType)
        {
            case ITEMTYPE.ITEM_ETC:
                invenUI.ShowItemInfo(etcData);
                break;
            case ITEMTYPE.ITEM_COMSUME:
                invenUI.ShowItemInfo(consumeData);
                break;
            case ITEMTYPE.ITEM_WEAPON:
                invenUI.ShowItemInfo(weaponData);
                break;
        }
    }
}
