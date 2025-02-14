using PlayerEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePlayerActionButton : MonoBehaviour
{
    [SerializeField] ACTION_TYPE actionType;
    
    public void PressButton()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetPlayerUpgradeUI.SetTexts(actionType);
    }
}
