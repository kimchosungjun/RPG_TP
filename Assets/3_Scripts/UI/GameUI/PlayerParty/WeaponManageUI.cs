using UnityEngine;
using UnityEngine.UI;

public class WeaponManageUI : MonoBehaviour, IPlayerPartyUI
{
    [SerializeField, Tooltip("0:Frame, 1:ScrollBar")] Image[] weaponManageImages;
    [SerializeField] WeaponManageSlot[] slots;
    [SerializeField] SelectWeaponLevelupButton levelupButton;
    [SerializeField] ReplaceWeaponButton replaceButton;
    [SerializeField] WeaponUpgradeView upgradeView;
    public void Init()
    {
        if (levelupButton == null) levelupButton = GetComponentInChildren<SelectWeaponLevelupButton>();
        if (replaceButton == null) replaceButton = GetComponentInChildren<ReplaceWeaponButton>();
        if(upgradeView==null) upgradeView= GetComponentInChildren<WeaponUpgradeView>(); 
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        weaponManageImages[0].sprite = res.GetSpriteAtlas("Slot_Atlas", "Weapon_Manage_Frame");
        weaponManageImages[1].sprite = res.GetSpriteAtlas("Slot_Atlas", "Scroll_Bar");
        levelupButton.Init();
        replaceButton.Init();

        Sprite buttonFrameSprite = res.GetSpriteAtlas("Button_Atlas", "WeaponManage_Slot_Button");
        int cnt = slots.Length;
        for (int i = 0; i < cnt; i++)
        {
            slots[i].Init(buttonFrameSprite);
        }

        upgradeView.Init(); 
    }

    public void TurnOff()
    {
        //throw new System.NotImplementedException();
    }

    public void TurnOn()
    {
        //throw new System.NotImplementedException();
    }
}
