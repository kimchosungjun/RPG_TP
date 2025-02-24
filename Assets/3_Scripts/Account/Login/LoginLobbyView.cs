using UnityEngine;
using UnityEngine.UI;
using UtilEnums;

public class LoginLobbyView : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject lobbyViewParent;
    [SerializeField] GameObject decideExitView;
    [SerializeField] GameObject logOutView;

    [Header("UI Images")]
    [SerializeField, Tooltip("0:Start, 1:Exit, 2:Logout")] Image[] buttonImages;
    [SerializeField, Tooltip("0:Button, 1:Button")] Image[] exitImages;
    [SerializeField, Tooltip("0:Button, 1:Button")] Image[] logoutImages;
    [SerializeField, Tooltip("0:Line, 1:Frame, 2:WarnFrame, 3:WarnButton")] Image[] serverIndicatorImages;
    [SerializeField] GameObject overFlowServerIndicator;
    [SerializeField] Button gameStartBtn;
    [SerializeField] Text gameStartText;

    bool onceSetImage = false;

    [SerializeField, Header("Connect Text")] PeriodAnimation connectText;
    public PeriodAnimation GetPeriodAnimation { get { return connectText; } }
    
    public void Init()
    {
        if (lobbyViewParent.activeSelf) lobbyViewParent.gameObject.SetActive(false);
        if (decideExitView.activeSelf) decideExitView.gameObject.SetActive(false);
        if (logOutView.activeSelf) logOutView.gameObject.SetActive(false);
        if(onceSetImage==false)
            SetImage();

        connectText.Init();
    }

    public void ActiveView() 
    {
        lobbyViewParent.SetActive(true);
    }

    public void SetImage()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        onceSetImage = true;
        Sprite buttonSprite = res.GetSpriteAtlas("Button_Atlas", "Red_Frame");
        buttonImages[0].sprite = res.GetSpriteAtlas("Button_Atlas", "StartGame_Frame"); 
        buttonImages[1].sprite = res.GetSpriteAtlas("Icon_Atlas", "Door_Icon"); 
        buttonImages[2].sprite = res.GetSpriteAtlas("Icon_Atlas", "Back_Icon"); 
        exitImages[0].sprite = buttonSprite;
        exitImages[1].sprite = buttonSprite;
        logoutImages[0].sprite = buttonSprite;
        logoutImages[1].sprite = buttonSprite;
        serverIndicatorImages[0].sprite = res.GetSpriteAtlas("Bar_Atlas", "HP_Line");
        serverIndicatorImages[1].sprite = res.GetSpriteAtlas("Bar_Atlas", "ServerBar");
        serverIndicatorImages[2].sprite = res.GetSpriteAtlas("Window_Atlas", "WeaponUpgrade_Frame");
        serverIndicatorImages[3].sprite = res.GetSpriteAtlas("Button_Atlas", "Red_Frame");
    }

    #region Btn Function
    public void PressGameStart()
    {
        gameStartBtn.interactable = false;
        gameStartText.color = gameStartBtn.colors.disabledColor;
        SharedMgr.UIMgr.LoginUICtrl.GetLoginFadeInView.SetBlock(true);
        connectText.StartPeriodAnimation(DecideEnterGame);
        SharedMgr.SoundMgr.PressButtonSFX();
    }

    public void DecideEnterGame() { SharedMgr.PhotonMgr.JoinRoom(); }

    public void FailJoinServer()
    {
        OverFlowServer();
        connectText.EndPeriodAnimation();
        gameStartBtn.interactable = true;
        gameStartText.color = gameStartBtn.colors.normalColor;
        SharedMgr.UIMgr.LoginUICtrl.GetLoginFadeInView.SetBlock(false);
    }

    public void PressGameExit()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        decideExitView.SetActive(true);
        SharedMgr.UIMgr.LoginUICtrl.GetSceneCtrl.SetActiveEditWindow = true;
    }

    public void PressLogOut()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        logOutView.SetActive(true);
        SharedMgr.SaveMgr.ClearUserData();
    }

    public void DecideGameExit()
    {
#if UNITY_EDITOR 
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.LoginUICtrl.GetSceneCtrl.SetActiveEditWindow = false;
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.LoginUICtrl.GetSceneCtrl.SetActiveEditWindow = false;
        Application.Quit();
#endif
    }

    public void CancelGameExit()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        decideExitView.SetActive(false);
        SharedMgr.UIMgr.LoginUICtrl.GetSceneCtrl.SetActiveEditWindow = false;
    }

    public void DecideLogOut()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.SceneMgr.SetPlayerAccount("ID", string.Empty);
        SharedMgr.SceneMgr.SetPlayerAccount("PW", string.Empty);
        SharedMgr.SaveMgr.ClearUserData();
        GetComponentInParent<LoginUICtrl>()?.ReturnLogin();        
    }

    public void CancelLogOut()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        logOutView.SetActive(false);
    }

    public void OverFlowServer() 
    {
        if(overFlowServerIndicator.activeSelf == false)
            overFlowServerIndicator.SetActive(true); 
    }

    public void Confirm()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        overFlowServerIndicator.SetActive(false);
    }
    #endregion
}
