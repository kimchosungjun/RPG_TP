using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManageUI : MonoBehaviour, IPlayerPartyUI
{
    [SerializeField] GameObject[] weaponManageParents;
    [SerializeField, Tooltip("0:Frame, 1:ScrollBar")] Image[] weaponManageImages;
    [SerializeField] SelectWeaponLevelupButton levelupButton;
    [SerializeField] ReplaceWeaponButton replaceButton;
    [SerializeField] WeaponManageView manageView;
    [SerializeField] WeaponUpgradeView upgradeView;

    public WeaponManageView GetManageView { get { return manageView; } }    

    public void Init()
    {
        if (levelupButton == null) levelupButton = GetComponentInChildren<SelectWeaponLevelupButton>();
        if (replaceButton == null) replaceButton = GetComponentInChildren<ReplaceWeaponButton>();
        if (manageView==null) manageView = GetComponentInChildren<WeaponManageView>();  
        if (upgradeView==null) upgradeView= GetComponentInChildren<WeaponUpgradeView>(); 
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

        manageView.Init(buttonFrameSprite);
        upgradeView.Init(); 
    }

    public void TurnOn()
    {
        manageView.SetDataToWeaponList();
        weaponManageParents[0].SetActive(true);
        if (weaponManageParents[1].activeSelf)
            weaponManageParents[1].SetActive(false);
    }

    public void TurnOff()
    {
        weaponManageParents[0].SetActive(false);
        weaponManageParents[1].SetActive(false);
    }
}
