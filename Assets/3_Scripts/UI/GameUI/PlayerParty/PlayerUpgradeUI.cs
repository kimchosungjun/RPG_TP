using PlayerEnums;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeUI : MonoBehaviour, ITurnOnOffUI
{
    [SerializeField, Tooltip("0:Normal, 1:Skill, 2:Ultimate, 3 : Upgrade Frame")] Image[] frames;
    [SerializeField, Tooltip("0:Normal, 1:Skill, 2:Ultimate, 3: LevelupBtn, 4:LevelupGold")] Image[] icons;
    [SerializeField, Tooltip("0: Name, 1:Lv, 2:Description")] Text[] curTexts;
    [SerializeField, Tooltip("0: Name, 1:Lv, 2:Description")] Text[] nextTexts;
    [SerializeField] Text goldText;
    [SerializeField, Tooltip("0:All, 1:NextLevelFrame")] GameObject upgradeParent;
    [SerializeField] Button levelUpButton;

    int useGold = 0;
    PlayerStat stat = null;
    PlayerBaseActionSOData actionSoData = null;
    ACTION_TYPE currentType;
    public void Init()
    {
        SetImages();
        if (upgradeParent.activeSelf)
            upgradeParent.SetActive(false);
        ClearTexts();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite actionBtnFrame = res.GetSpriteAtlas("Slot_Atlas", "Item_Icon_Frame");
        frames[0].sprite = actionBtnFrame;
        frames[1].sprite = actionBtnFrame;
        frames[2].sprite = actionBtnFrame;
        frames[3].sprite = res.GetSpriteAtlas("Bar_Atlas_2", "InvenDesc_Bar");


        icons[0].sprite = res.GetSpriteAtlas("Icon_Atlas", "Attack_Icon");
        icons[1].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Skill_Icon");
        icons[2].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Ultimate_Icon");
        icons[3].sprite = res.GetSpriteAtlas("Bar_Atlas_2", "Red_Long_Bar");
        icons[4].sprite = res.GetSpriteAtlas("Icon_Atlas_2", "Gold_Icon");
    }
    public void TurnOn()
    {
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetCharacterSlotSetUI.TurnOn();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ManageGoldUI(true);
        if (upgradeParent.activeSelf == false)
            upgradeParent.SetActive(true);
    }

    public void ClearTexts()
    {
        for (int i = 0; i < 3; i++)
        {
            curTexts[i].gameObject.SetActive(false);
            nextTexts[i].gameObject.SetActive(false);
        }
        levelUpButton.interactable = false;
        goldText.text = "0";   
    }

    public void SetTexts(ACTION_TYPE _playerActionType)
    {
        if (stat == null) return;
        PlayerBaseActionSOData actionData = stat.GetActionSoData(_playerActionType);
        if (actionData == null) return;
        actionSoData = actionData;
        currentType = _playerActionType;
        curTexts[0].text = actionData.GetActionName;
        curTexts[1].text = "Lv. " +actionData.GetCurrentLevel;
        curTexts[2].text = actionData.GetActionDescription;
           
        if (actionData.CanLevelUp() == false)
        {
            for (int i = 0; i < 3; i++)
            {
                curTexts[i].gameObject.SetActive(true);
                nextTexts[i].gameObject.SetActive(false);
            }
            levelUpButton.interactable = false;
            goldText.text = "0";
            useGold = 0;
        }
        else
        {
            Tuple<string, int, string> nextLevelData = actionData.GetNextLevelData();
            nextTexts[0].text = nextLevelData.Item1;
            nextTexts[1].text = "Lv. " + nextLevelData.Item2;
            nextTexts[2].text = nextLevelData.Item3;
            for (int i = 0; i < 3; i++)
            {
                curTexts[i].gameObject.SetActive(true);
                nextTexts[i].gameObject.SetActive(true);
            }
            levelUpButton.interactable = true;
            useGold = actionData.GetNextLevelUpGold();
            goldText.text = useGold.ToString();
        }
    }

    public void UpdateLvUpData()
    {
        SetTexts(currentType);
    }

    public void TurnOff()
    {
        if (upgradeParent.activeSelf)
            upgradeParent.SetActive(false);
        stat = null;
        actionSoData = null;
    }

    public void ChangeCharacter(int _characterID)
    {
        stat = SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat(_characterID);
        ClearTexts();
    }

    public void PressLevelUpButton()
    {
        SharedMgr.InventoryMgr.RemoveGold(useGold);
        SharedMgr.UIMgr.GameUICtrl.GetUpgradeIndicatorUI.TurnOn(actionSoData);
        //if (SharedMgr.InventoryMgr.CanUseGold(useGold))
        //{
        //    SharedMgr.InventoryMgr.RemoveGold(useGold);
        //    SharedMgr.UIMgr.GameUICtrl.GetUpgradeIndicatorUI.TurnOn(actionSoData);
        //}
        //else
        //{
        //    Debug.Log("Don't have enough money!!!");
        //}
    }
}
