using UnityEngine;
using UnityEngine.UI;
using ItemEnums;

public class InventorySlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:TextFame, 2:Icon")] Image[] slotImages;
    [SerializeField] Text numberText;
    [SerializeField] Button slotButton;
    EtcData etcData = null;
    ConsumeData consumeData = null;
    WeaponData weaponData = null;

    Color transparentColor = Color.white;
    Color defaultColor = Color.white;

    public void SetSlotImage()
    {
        defaultColor = slotImages[2].color;
        transparentColor = defaultColor;
        transparentColor.a = 0f;

        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas","InvenSlot_Frame");
        slotImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Black_Frame");
    }

    public void ChangeSlot()
    {
        slotImages[2].color = transparentColor;
        numberText.text = string.Empty;
        slotButton.interactable = false;
    }

    public void ChangeSlot(EtcData _etcData)
    {
        slotImages[2].sprite = _etcData.itemIcon;
        slotImages[2].color = defaultColor;
        numberText.text = _etcData.itemCnt+"개";
        etcData = _etcData;
        slotButton.interactable = true;
    }

    public void ChangeSlot(ConsumeData _consumeData)
    {
        slotImages[2].sprite = _consumeData.itemIcon;
        slotImages[2].color = defaultColor;
        numberText.text = _consumeData.itemCnt + "개";
        consumeData = _consumeData;
        slotButton.interactable = true;
    }

    public void ChangeSlot(WeaponData _weaponData)
    {
        slotImages[2].sprite = _weaponData.itemIcon;
        slotImages[2].color = defaultColor;
        numberText.text = "Lv."+_weaponData.weaponCurrentLevel;
        weaponData = _weaponData;
        slotButton.interactable = true;
    }

    public void PressSlot()
    {
        InventoryUI invenUI = SharedMgr.UIMgr.GameUICtrl.GetInventoyUI;
        switch (invenUI.GetCurrentType)
        {
            case ITEMTYPE.ITEM_ETC:
                invenUI.CurrentItemData = etcData;
                invenUI.ShowItemInfo(etcData);
                break;
            case ITEMTYPE.ITEM_COMSUME:
                invenUI.CurrentItemData = consumeData;
                invenUI.ShowItemInfo(consumeData);
                break;
            case ITEMTYPE.ITEM_WEAPON:
                invenUI.CurrentItemData = weaponData;
                invenUI.ShowItemInfo(weaponData);
                break;
        }
        invenUI.GetDeleteUI.InActive();
    }

}
