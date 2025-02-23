using UnityEngine;
using UnityEngine.UI;

public class GameExitUI : UIBase
{
    bool isActive =false;
    [SerializeField] GameObject gameExitWindow;
    [SerializeField, Tooltip("0:Frame, 1:YesBtn, 2:NoBtn")] Image[] frameImages;

    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite redBtnSprite = res.GetSpriteAtlas("Button_Atlas", "Red_Frame");
        frameImages[0].sprite = res.GetSpriteAtlas("Window_Atlas", "Popup_Frame");
        frameImages[1].sprite = redBtnSprite;
        frameImages[2].sprite = redBtnSprite;
    }

    public override void InputKey()
    {
        if (isActive)
        {
            isActive = false;
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.NONE;
            gameExitWindow.SetActive(false);
            return;
        }
        gameExitWindow.SetActive(true);
        isActive = true;
        SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.EXIT;
    }

    public void PressConfirmBtn()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        gameExitWindow.SetActive(false);
        SaveAccountData();
    }

    public void PressCancelBtn()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        InputKey();
    }

    public void SaveAccountData()
    {
        SharedMgr.UIMgr.GameUICtrl.GetEtcUI.ShowSaveDataUI();
        SharedMgr.SaveMgr.SavePlayerData(ReturnToLobbyScene);
    }

    public void ReturnToLobbyScene() { SharedMgr.PhotonMgr.LeaveRoom();  }

}
