using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    #region Variable
    [SerializeField] GameObject settingWindow;
    [SerializeField] GraphicControlUI graphicControlUI;
    [SerializeField] SoundControlUI soundControlUI;
    [SerializeField, Tooltip("0:Graphic, 1:Sound, 2:Back")] Image[] btnImages;
    [SerializeField] Text curText;
    [SerializeField] Animator anim;
    ITurnOnOffUI[] settingUIs;

    Setting curState = Setting.Graphic;
    Setting CurState
    {
        set 
        { 
            curState = value;
            ChangeState(value);
        }
    } 

    public enum Setting
    {
        Graphic=0,
        Sound
    }

    public void ChangeState(Setting _curSetting)
    {
        switch (_curSetting)
        {
            case Setting.Graphic:
                curText.text = "그래픽";
                settingUIs[(int)Setting.Graphic].TurnOn();
                settingUIs[(int)Setting.Sound].TurnOff();
                break;
            case Setting.Sound:
                curText.text = "사운드";
                settingUIs[(int)Setting.Graphic].TurnOff();
                settingUIs[(int)Setting.Sound].TurnOn();
                break;
        }
    }
    #endregion

    #region Setting UI 
    public void Init()
    {
        soundControlUI.Init();
        graphicControlUI.Init();
        SetButtonImage();   
        settingUIs = new ITurnOnOffUI[2];
        settingUIs[0] = graphicControlUI;
        settingUIs[1] = soundControlUI;
        ChangeState(Setting.Graphic);
    }

    public void SetButtonImage()
    {
        Sprite btnImage = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Black_Frame");
        btnImages[0].sprite = btnImage;
        btnImages[1].sprite = btnImage;
        btnImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
    }

    public override void InputKey()
    {
        if (isActiveState)
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
            anim.Play("Idle");
            settingWindow.SetActive(false);
            isActiveState = false;
            SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PopUIPopup();
            return;
        }
        
        isActiveState = true;
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true);
        settingWindow.SetActive(true);
        SharedMgr.UIMgr.GameUICtrl.GetUIBaseControl.PushUIPopup(this);
        anim.Play("SettingOpen");
    }

    #endregion

    #region Press Btn
    public void PressGraphicBtn()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        CurState = Setting.Graphic;
    }
    
    public void PressSoundBtn()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        CurState = Setting.Sound;
    }

    public void PressBackBtn()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        InputKey();
    }
    #endregion
}
