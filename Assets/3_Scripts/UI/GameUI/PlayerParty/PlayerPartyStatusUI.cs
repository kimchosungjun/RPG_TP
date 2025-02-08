using PlayerTableClassGroup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPartyStatusUI : MonoBehaviour, ITurnOnOffUI
{
    [SerializeField] GameObject uiFrameObject;
    [SerializeField, Tooltip("0:HP, 1:Atk, 2:Def, 3:Cri, 4:PlayerIcon")] Image[] icons;
    [SerializeField, Tooltip("0:Hp, 1:Atk, 2:Def, 3:Cir, 4:Player")] Image[] iconFrames;
    [SerializeField] PlayerPartyStatusButton[] buttons;
    [SerializeField, Tooltip("0:HP, 1:Atk, 2:Def, 3:Cri, 4:Lv, 5:Name, 6 :ClassText")] Text[] characterStatTexts;
    [SerializeField] Slider expSlider;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        icons[0].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "HP_Icon");
        icons[1].sprite = res.GetSpriteAtlas("Icon_Atlas", "Weapon_Icon");
        icons[2].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Defence_Icon");
        icons[3].sprite = res.GetSpriteAtlas("Icon_Atlas", "Critical_Icon");

        Sprite slotSprite = res.GetSpriteAtlas("Bar_Atlas_2", "Delete_Bar");
        iconFrames[0].sprite = slotSprite;
        iconFrames[1].sprite = slotSprite;
        iconFrames[2].sprite = slotSprite;
        iconFrames[3].sprite = slotSprite;
        iconFrames[4].sprite = res.GetSpriteAtlas("Bar_Atlas_3", "PartyPlayerStatus_Frame");

        int slotCnt = buttons.Length;
        for (int i = 0; i < slotCnt; i++)
        {
            buttons[i].Init();
        }
    }

    public void PressCharacter(int _id)
    {
        List<BasePlayer> players = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers;
        int cnt =players.Count;
        for(int i=0; i<cnt; i++)
        {
            if (players[i].PlayerStat.GetSaveStat.playerTypeID == _id)
            {
                UpdatePlayerStat(players[i].PlayerStat);
                return;
            }
        }
    }

    public void TurnOn()
    {
        if (uiFrameObject.activeSelf == false)
            uiFrameObject.SetActive(true);
        SharedMgr.UIMgr.GameUICtrl.GetModelCam.TurnOn();
        UpdateCharacterButton();
        buttons[0].PressButton();

        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetCharacterSlotSetUI.TurnOff();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ManageGoldUI(false);
    }

    public void TurnOff()
    {
        SharedMgr.UIMgr.GameUICtrl.GetModelCam.TurnOff();
        if (uiFrameObject.activeSelf)
            uiFrameObject.SetActive(false);
    }
    
    public void UpdatePlayerStat(PlayerStat _stat)
    {
        PlayerLevelTableData lvTable = SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData();
        int curLv = _stat.GetSaveStat.currentLevel;
        characterStatTexts[0].text = _stat.MaxHP.ToString();
        characterStatTexts[1].text = _stat.Attack.ToString();
        characterStatTexts[2].text = _stat.Defence.ToString();
        characterStatTexts[3].text = _stat.Critical * 100 + "%";
        characterStatTexts[4].text = "Lv. "+ curLv + "/" + lvTable.maxLevel;
        characterStatTexts[5].text = _stat.GetActorName;

        #region Class
        // Later Revise 

        switch (_stat.GetBattleType)
        {
            case PlayerEnums.BATTLE_TYPE.NEAR:
                icons[4].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Attack_Icon");
                characterStatTexts[6].text = "근거리 전투";
                break;
            case PlayerEnums.BATTLE_TYPE.FAR:
                icons[4].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Far_Icon");
                characterStatTexts[6].text = "원거리 전투";
                break;
        }
        #endregion

        if (lvTable.GetNeedExp(curLv)==-1)
        {
            expSlider.maxValue = 1f;
            expSlider.value = 1f;
        }
        else
        {
            expSlider.maxValue = lvTable.GetNeedExp(curLv);
            expSlider.value = _stat.GetSaveStat.currentExp;
        }
        SharedMgr.UIMgr.GameUICtrl.GetModelCam.ChangeModel(_stat.GetSaveStat.playerTypeID);
    }

    public void UpdateCharacterButton()
    {
        PlayerStat stat = null;
        ResourceMgr res = SharedMgr.ResourceMgr;
        List<BasePlayer> players = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers;
        int slotCnt = buttons.Length;
        int playerCnt = players.Count;
        for (int i = 0; i < playerCnt; i++)
        {
            if (i > slotCnt - 1) break;
            stat = players[i].PlayerStat;
            int id = stat.GetSaveStat.playerTypeID;
            buttons[i].SetButton(id,res.GetSpriteAtlas(stat.GetAtlasName,stat.GetFileName));
        }
    }
}
