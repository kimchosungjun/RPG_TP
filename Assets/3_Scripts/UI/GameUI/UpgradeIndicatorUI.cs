using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIndicatorUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Levelup GradientFrame, 1:Direction, 2:Division")] Image[] images;
    [SerializeField] PlayerUpgradeIndicateSlot[] slots;
    [SerializeField] PlayerUpgradeIndicateSlot actionSlot;
    [SerializeField, Tooltip("0:CurLv, 1:NextLv")] Text[] texts;
    [SerializeField] GameObject upgradeIndicatorParent;

    string LV = "LV. "; 
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite slotFrame = res.GetSpriteAtlas("Bar_Atlas", "Gradient_Line");
        Sprite nextIcon = res.GetSpriteAtlas("Icon_Atlas", "Next_Icon");
        images[0].sprite = slotFrame;
        images[1].sprite = nextIcon;
        images[2].sprite = res.GetSpriteAtlas("Bar_Atlas", "Bot_Under_Division");

        int slotCnt = slots.Length;
        for(int i=0; i<slotCnt; i++)
        {
            slots[i].Init(slotFrame, nextIcon);
        }
        actionSlot.Init(slotFrame, nextIcon);
    }

    public void TurnOn(WeaponData _data)
    {
        //texts[0].text = LV + _currentLevel;
        //texts[1].text = LV + _currentLevel + 1;
        // To Do ~~
        upgradeIndicatorParent.SetActive(true);
    }

    public void TurnOn(PlayerBaseActionSOData _soData)
    {
        Tuple<string,int,string> nextLvData = _soData.GetNextLevelData();
        upgradeIndicatorParent.SetActive(true);
        texts[0].text = LV + _soData.GetCurrentLevel;
        texts[1].text = LV +  nextLvData.Item2;
        actionSlot.SetText(_soData.GetActionDescription, nextLvData.Item3, nextLvData.Item1);
        _soData.LevelUp();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetPlayerUpgradeUI.UpdateLvUpData();
    }

    public void TurnOff()
    {
        upgradeIndicatorParent.SetActive(false);
    }
}
