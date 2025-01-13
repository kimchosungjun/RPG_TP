using UnityEngine;
using UnityEngine.UI;

public class WeaponManageUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:ScrollBar")] Image[] weaponManageImages;
    [SerializeField] WeaponManageSlot[] slots;
    [SerializeField] SelectWeaponLevelupButton levelupButton;
    [SerializeField] ReplaceWeaponButton replaceButton;

    public void Init()
    {
        if(levelupButton==null) levelupButton=GetComponentInChildren<SelectWeaponLevelupButton>();   
        if(replaceButton == null) replaceButton=GetComponentInChildren<ReplaceWeaponButton>();  
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        weaponManageImages[0].sprite = res.GetSpriteAtlas("Slot_Atlas", "Weapon_Manage_Frame");
        weaponManageImages[1].sprite = res.GetSpriteAtlas("Slot_Atlas", "Scroll_Bar");
        levelupButton.Init();
        replaceButton.Init();   
        int cnt = slots.Length;
        for(int i=0; i<cnt; i++)
        {
            slots[i].Init();
        }
    }
}
