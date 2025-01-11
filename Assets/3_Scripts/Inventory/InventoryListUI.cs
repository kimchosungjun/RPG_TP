using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemEnums;

public class InventoryListUI : MonoBehaviour
{
    [SerializeField] Text itemTypeText;
    [SerializeField] Text bagCntTex;
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField, Tooltip("0:Frame, 1:Icon")] Image[] deleteBtnImages;
    [SerializeField, Tooltip("0:Top Indicate Image, 1:Side Bar Image")] Image[] invenImages;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        invenImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas_2", "Inven_Bar");
        invenImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Slot_Atlas", "Scroll_Bar");
        deleteBtnImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Black_Frame");
        deleteBtnImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Trash_Icon");
        int slotCnt = inventorySlots.Length;
        for(int i=0; i<slotCnt; i++)
        {
            inventorySlots[i].SetSlotImage();
        }
    }

    public void ChangeItemType(ITEMTYPE _itemType)
    {
        int cnt = 0;
        switch (_itemType)
        {
            case (ITEMTYPE.ITEM_ETC):
                itemTypeText.text = "기타";
                List<EtcData> etcDats = SharedMgr.InventoryMgr.GetEtcInventory();
                cnt = etcDats.Count;
                ChangeSlots(etcDats);
                break;
            case (ITEMTYPE.ITEM_COMSUME):
                itemTypeText.text = "소모품";
                List<ConsumeData> consumeDats = SharedMgr.InventoryMgr.GetConsumeInventory();
                cnt = consumeDats.Count;
                ChangeSlots(consumeDats);
                break;
            case (ITEMTYPE.ITEM_WEAPON):
                itemTypeText.text = "무기";
                List<WeaponData> weaponDats = SharedMgr.InventoryMgr.GetWeaponInventory();
                cnt = weaponDats.Count;
                ChangeSlots(weaponDats);
                break;
        }

        bagCntTex.text = cnt + "/25";
    }

    public void ChangeSlots(List<EtcData> _etcDatas)
    {
        int dataCnt = _etcDatas.Count;  
        int slotCnt = inventorySlots.Length;
        for(int i=0; i< dataCnt; i++)
        {
            inventorySlots[i].ChangeSlot(_etcDatas[i]);
        }
        
        for(int i=dataCnt; i<slotCnt; i++)
        {
            inventorySlots[i].ChangeSlot();
        }
    }

    public void ChangeSlots(List<ConsumeData> _consumeDatas)
    {
        int dataCnt = _consumeDatas.Count;
        int slotCnt = inventorySlots.Length;
        for (int i = 0; i < dataCnt; i++)
        {
            inventorySlots[i].ChangeSlot(_consumeDatas[i]);
        }

        for (int i = dataCnt; i < slotCnt; i++)
        {
            inventorySlots[i].ChangeSlot();
        }
    }
    public void ChangeSlots(List<WeaponData> _weaponDatas)
    {
        int dataCnt = _weaponDatas.Count;
        int slotCnt = inventorySlots.Length;
        for (int i = 0; i < dataCnt; i++)
        {
            inventorySlots[i].ChangeSlot(_weaponDatas[i]);
        }

        for (int i = dataCnt; i < slotCnt; i++)
        {
            inventorySlots[i].ChangeSlot();
        }
    }
}
