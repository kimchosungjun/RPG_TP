using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIEnums;


public class PlayerPartyUI : MonoBehaviour, IInputKeyUI
{
    bool isActive = false;
    ITurnOnOffUI[] partyUISet;
    PARTY currentShowType = PARTY.STATUS;
    public PARTY GetCurrentUIType { get { return currentShowType; }  }
    
    [SerializeField] GameObject playerPartyUISetFrame;
    [SerializeField] PartySideBarUI partySideBarUI;
    [SerializeField] PlayerPartyStatusUI playerPartyStatusUI;
    [SerializeField] WeaponManageUI weaponManageUI;
    [SerializeField] PlayerUpgradeUI playerUpgradeUI;
    [SerializeField] CharacterSlotSetUI characterSlotSetUI;
    [SerializeField] Image exitBtnImage;
    [SerializeField] InventoryGoldUI goldUI;

    //public PartySideBarUI GetPartySideBarUI { get { return partySideBarUI; } }
    public PlayerPartyStatusUI GetPlayerPartyStatusUI { get { return playerPartyStatusUI; } }
    public WeaponManageUI GetWeaponManageUI { get {return weaponManageUI; } }   
    public PlayerUpgradeUI GetPlayerUpgradeUI { get {return playerUpgradeUI; } }
    public CharacterSlotSetUI GetCharacterSlotSetUI { get { return characterSlotSetUI; } }
    public InventoryGoldUI GetInventoryGoldUI { get {   return goldUI; } }  

    List<IUpdateUI> updateUIList = new List<IUpdateUI>();

    [Header("Character Button Effect")]
    [SerializeField] RectTransform effectTransform;
    public RectTransform GetEffectTransform { get { return effectTransform; } }

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
        if(characterSlotSetUI ==null) characterSlotSetUI = GetComponentInChildren<CharacterSlotSetUI>();
        if (goldUI == null) goldUI = GetComponentInChildren<InventoryGoldUI>();
    }

    public void InitPartyUISet()
    {
        partyUISet = new ITurnOnOffUI[(int)PARTY.MAX];
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
        characterSlotSetUI.Init();
        exitBtnImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
        goldUI.Init();
    }
    #endregion

    #region Setup (Manage UI Active State)
    public void Setup()
    {
        updateUIList.Add(playerPartyStatusUI);
        updateUIList.Add(weaponManageUI);
        updateUIList.Add(playerUpgradeUI);

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
    public void InputKey()
    {
        isActive = !isActive;
        if (isActive)
        {
            Setup();
            if (playerPartyUISetFrame.activeSelf == false)
                playerPartyUISetFrame.SetActive(true);
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = GAMEUI.PLAYER_PARTY;

            if(currentShowType != PARTY.MAX)
                updateUIList[(int)currentShowType].UpdateData();
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

    public void InputUnderCharacterButton(int _characterID)
    {
        switch (currentShowType)
        {
            case PARTY.WEAPON:
                weaponManageUI.ChangeCharacter(_characterID);
                break;
            case PARTY.SKILL_UPGRADE:
                playerUpgradeUI.ChangeCharacter(_characterID);
                break;
        }
    }

    public void ManageGoldUI(bool _isActive)
    {
        if (goldUI.gameObject.activeSelf == _isActive) return;
        if (_isActive)
        {
            goldUI.UpdateGold();
            goldUI.gameObject.SetActive(true);
        }
        else
        {
            goldUI.gameObject.SetActive(false);
        }
    }

}
