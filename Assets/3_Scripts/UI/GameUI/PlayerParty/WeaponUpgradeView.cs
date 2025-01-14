using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeView : MonoBehaviour
{
    [SerializeField] WeaponUpgradeMatSlot matSlot;
    [SerializeField] Transform parentTransform;
    List<WeaponUpgradeMatSlot> slots = new List<WeaponUpgradeMatSlot>();

    [SerializeField, Tooltip("0:BackFrame, 1:EnhanceBtn, 2:ClearBtn, 3:AtkFrame, 4:AddFrame, 5:UseSlotFrame")] Image[] images;
    [SerializeField,Tooltip(" 0:AtkIcon, 1:AddIcon, 2:Direction, 3:AddDirection,4:UseSlotIcon") ] Image[] icons;
    [SerializeField, Tooltip("0:WeaponName, 1:curLv, 2:NextLv, 3:CurAtk, 4:NextAtk, 5:curAdd, 6:nextAdd")] Text[] texts;

    Sprite defaultUseItemIcon = null;
    public void Init()
    {
        UpdateMatSlots();
        SetImages();
    }

    public void UpdateMatSlots()
    {
        List<EtcData> datas =  SharedMgr.InventoryMgr.GetEtcInventory();
        int dataCnt = datas.Count;
        int slotCnt = slots.Count;

        if (slotCnt < dataCnt)
        {
            int diff = dataCnt  - slotCnt;  
            for(int i=0; i<diff; i++)
            {
                GameObject go = Instantiate(matSlot.gameObject, parentTransform);
                slots.Add(go.GetComponent<WeaponUpgradeMatSlot>());
            }
        }

        for(int i=0; i<dataCnt; i++)
        {
            slots[i].Init(datas[i]);
        }
    } 

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        images[0].sprite = res.GetSpriteAtlas("Window_Atlas", "WeaponUpgrade_Frame");
        images[1].sprite = res.GetSpriteAtlas("Bar_Atlas_2", "Red_Long_Bar");
        images[2].sprite = res.GetSpriteAtlas("Button_Atlas", "Black_Frame");
        Sprite indicateFrame = res.GetSpriteAtlas("Slot_Atlas", "Weapon_Manage_Frame");
        images[3].sprite = indicateFrame;
        images[4].sprite = indicateFrame;
        images[5].sprite = res.GetSpriteAtlas("Slot_Atlas", "Item_Icon_Frame");
        
        Sprite weaponIcon = res.GetSpriteAtlas("Icon_Atlas", "Weapon_Icon");
        Sprite directionIcon = res.GetSpriteAtlas("Icon_Atlas", "Next_Icon");
        icons[0].sprite = weaponIcon;
        icons[1].sprite = weaponIcon;
        icons[2].sprite = directionIcon;
        icons[3].sprite = directionIcon;

        defaultUseItemIcon = res.GetSpriteAtlas("Icon_Atlas_3", "Type_Etc");
        icons[4].sprite = defaultUseItemIcon;
    }
}
