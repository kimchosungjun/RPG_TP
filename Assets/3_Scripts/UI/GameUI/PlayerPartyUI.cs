using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIEnums;

public class PlayerPartyUI : MonoBehaviour
{
    bool isActive = false;
    PARTY currentShowType = PARTY.STATUS;

    [SerializeField] PartySideBarUI partySideBarUI;
    [SerializeField] PlayerPartyStatusUI playerPartyStatusUI;
    [SerializeField] WeaponManageUI weaponManageUI;
    [SerializeField] PlayerUpgradeUI playerUpgradeUI;
    [SerializeField] Image exitBtnImage;
    public PlayerPartyStatusUI GetPlayerPartyStatusUI { get { return playerPartyStatusUI; } }
    public WeaponManageUI GetWeaponManageUI { get {return weaponManageUI; } }   
    public PlayerUpgradeUI GetPlayerUpgradeUI { get {return playerUpgradeUI; } }    
    public void Init()
    {
        Link();
        InitChildrenUI();
    }

    public void Link()
    {
        if(partySideBarUI==null) partySideBarUI = GetComponentInChildren<PartySideBarUI>();   
        if(playerPartyStatusUI==null) playerPartyStatusUI = GetComponentInChildren<PlayerPartyStatusUI>();    
        if(weaponManageUI == null) weaponManageUI= GetComponentInChildren<WeaponManageUI>();    
        if (playerUpgradeUI==null)  playerUpgradeUI= GetComponentInChildren<PlayerUpgradeUI>(); 
    }

    public void InitChildrenUI()
    {
        partySideBarUI.Init();
        playerPartyStatusUI.Init();
        weaponManageUI.Init();
        playerUpgradeUI.Init();
        exitBtnImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
    }

    public void InputPartyKey()
    {
        isActive = !isActive;
        if (isActive)
        {
         //   UpdateQuestDatas();
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = GAMEUI.PLAYER_PARTY;
        }
        else
        {
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = GAMEUI.NONE;
        }
    }
}
