using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeUI : MonoBehaviour, IPlayerPartyUI
{
    [SerializeField, Tooltip("0:Normal, 1:Skill, 2:Ultimate, 3 : Upgrade Frame")] Image[] frames;
    [SerializeField, Tooltip("0:Normal, 1:Skill, 2:Ultimate, 3: LevelupBtn, 4:LevelupGold")] Image[] icons;

    public void Init()
    {
        SetImages();
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
    }

    public void TurnOff()
    {
        //throw new System.NotImplementedException();
    }
}
