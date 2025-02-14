using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySideBarUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:frame, 1:user, 2:weapon,3:upgrade")] Image[] sideBarImages;

    #region Init Set Image 
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        sideBarImages[0].sprite = res.GetSpriteAtlas("Slot_Atlas", "SideSlot_Frame");
        sideBarImages[1].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "User_Icon");
        sideBarImages[2].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Weapon_Icon");
        sideBarImages[3].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Upgrade_Icon");
    }
    #endregion

    #region Press Button
    public void PressPlayerStatus()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ChangeUI(UIEnums.PARTY.STATUS);
    }

    public void PressWeapon()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ChangeUI(UIEnums.PARTY.WEAPON);
    }

    public void PressUpgrade()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.ChangeUI(UIEnums.PARTY.SKILL_UPGRADE);
    }
    #endregion
}
