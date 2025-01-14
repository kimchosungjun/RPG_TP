using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIEnums;

public interface IPlayerPartyUI
{
    public void TurnOn();
    public void TurnOff();
}

public class PlayerPartyUI : MonoBehaviour
{
    bool isActive = false;
    IPlayerPartyUI[] partyUISet;
    PARTY currentShowType = PARTY.STATUS;
    
    [SerializeField] GameObject playerPartyUISetFrame;
    [SerializeField] PartySideBarUI partySideBarUI;
    [SerializeField] PlayerPartyStatusUI playerPartyStatusUI;
    [SerializeField] WeaponManageUI weaponManageUI;
    [SerializeField] PlayerUpgradeUI playerUpgradeUI;
    [SerializeField] Image exitBtnImage;

    //public PartySideBarUI GetPartySideBarUI { get { return partySideBarUI; } }
    public PlayerPartyStatusUI GetPlayerPartyStatusUI { get { return playerPartyStatusUI; } }
    public WeaponManageUI GetWeaponManageUI { get {return weaponManageUI; } }   
    public PlayerUpgradeUI GetPlayerUpgradeUI { get {return playerUpgradeUI; } }

    #region Init UI 
    public void Init()
    {
        Link();
        InitPartyUISet();
        InitChildrenUI();
        //Setup();
    }

    public void Link()
    {
        if(partySideBarUI==null) partySideBarUI = GetComponentInChildren<PartySideBarUI>();   
        if(playerPartyStatusUI==null) playerPartyStatusUI = GetComponentInChildren<PlayerPartyStatusUI>();    
        if(weaponManageUI == null) weaponManageUI= GetComponentInChildren<WeaponManageUI>();    
        if (playerUpgradeUI==null)  playerUpgradeUI= GetComponentInChildren<PlayerUpgradeUI>(); 
    }

    public void InitPartyUISet()
    {
        partyUISet = new IPlayerPartyUI[(int)PARTY.MAX];
        partyUISet[0] = playerPartyStatusUI;
        partyUISet[1] = weaponManageUI;
        partyUISet[2] = playerUpgradeUI;
    }

    public void InitChildrenUI()
    {
        partySideBarUI.Init();
        playerPartyStatusUI.Init();
        weaponManageUI.Init();
        playerUpgradeUI.Init();
        exitBtnImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
    }
    #endregion

    #region Setup (Manage UI Active State)
    public void Setup()
    {
        ManageActiveUI();
    }

    public void ManageActiveUI()
    {
        int cnt = partyUISet.Length;
        int curState = (int)currentShowType;
        for (int i = 0; i < cnt; i++)
        {
            if (i == curState)
                partyUISet[i].TurnOn();
            else
                partyUISet[i].TurnOff();
        }
    }

    #endregion

    // Game UI Ctrl
    public void InputPartyKey()
    {
        isActive = !isActive;
        if (isActive)
        {
            Setup();
            if (playerPartyUISetFrame.activeSelf == false)
                playerPartyUISetFrame.SetActive(true);
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = GAMEUI.PLAYER_PARTY;
        }
        else
        {
            if (playerPartyUISetFrame.activeSelf)
                playerPartyUISetFrame.SetActive(false);
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = GAMEUI.NONE;
        }
    }

    // Side Bar 
    public void ChangeUI(PARTY _changeUI)
    {
        currentShowType = _changeUI;
        ManageActiveUI();
    }

}
