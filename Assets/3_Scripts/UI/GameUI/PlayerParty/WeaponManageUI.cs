using ItemEnums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManageUI : MonoBehaviour, ITurnOnOffUI, IUpdateUI
{
    [SerializeField] GameObject[] weaponManageParents;
    [SerializeField, Tooltip("0:Frame, 1:ScrollBar")] Image[] weaponManageImages;
    [SerializeField] SelectWeaponLevelupButton levelupButton;
    [SerializeField] ReplaceWeaponButton replaceButton;
    [SerializeField] WeaponManageView manageView;
    [SerializeField] WeaponUpgradeView upgradeView;

    List<WeaponData> weapons = null;
    public WeaponManageView GetManageView { get { return manageView; } }

    int changeCharacterID = -1;

    #region Select Data
    int currentSelectCharacterID = -1;
    public int GetCurrentSelectCharacterID { get { return currentSelectCharacterID; } }

    [SerializeField] WeaponData currentSelectWeaponData = null;
    public WeaponData CurrentSelectWeaponData { get { return currentSelectWeaponData; }  set { currentSelectWeaponData = value; } }
    #endregion

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
        weaponManageParents[0].SetActive(true);
        if (weaponManageParents[1].activeSelf)
            weaponManageParents[1].SetActive(false);
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetCharacterSlotSetUI.TurnOn();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ManageGoldUI(true);
    }

    public void TurnOff()
    {
        manageView.ClearWeaponInfos();
        weaponManageParents[0].SetActive(false);
        weaponManageParents[1].SetActive(false);
    }

    public void ChangeCharacter(int _id)
    {
        changeCharacterID = _id;
        if (SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(_id) == null) return;
        WEAPONTYPE type = SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(_id).GetWeaponType();
        currentSelectCharacterID = _id;

        weapons = SharedMgr.InventoryMgr.GetSortWeaponGroup(type);
        manageView.SetDataToWeaponList(weapons);
        manageView.ClearWeaponInfos();
    }

    public void ChangeHoldWeapon()
    {
        PlayerStat stat = SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat(currentSelectCharacterID);
        if (stat == null) return;

        WeaponData holdWeapon = SharedMgr.InventoryMgr.GetHoldWeaponData(stat.HoldWeaponUniqueID);
        stat.HoldWeaponUniqueID = currentSelectWeaponData.uniqueID;
        SharedMgr.InventoryMgr.ChangeHoldWeapon(holdWeapon, currentSelectWeaponData, currentSelectCharacterID);
        manageView.PressWeaponSlot(currentSelectWeaponData);
    }

    public void SelectLevelUpButton()
    {
        if (weaponManageParents[0].activeSelf)
            weaponManageParents[0].SetActive(false);
        if (weaponManageParents[1].activeSelf == false)
            weaponManageParents[1].SetActive(true);
        upgradeView.UpdateMatSlots();
        upgradeView.SetWeaponData(currentSelectWeaponData);
    }

    public void UpdateData()
    {
        if (changeCharacterID == -1) return;
        ChangeCharacter(changeCharacterID);
    }
}
