using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UtilEnums;

public class LoginLobbyView : MonoBehaviour
{
    [SerializeField] GameObject lobbyViewParent;
    [SerializeField] GameObject decideExitView;
    [SerializeField] GameObject logOutView;

    [SerializeField, Tooltip("0:Start, 1:Exit, 2:Logout")] Image[] buttonImages;
    [SerializeField, Tooltip("0:Button, 1:Button")] Image[] exitImages;
    [SerializeField, Tooltip("0:Button, 1:Button")] Image[] logoutImages;
    [SerializeField, Tooltip("0:Line, 1:Frame, 2:WarnFrame, 3:WarnButton")] Image[] serverIndicatorImages;
    [SerializeField, Tooltip("0:State, 1:Cnt")] Text[] serverTexts;
    [SerializeField] GameObject overFlowServerIndicator;
    string[] serverStateTexts = new string[5];
    [SerializeField] Color[] serverStateColors;
    bool onceSetImage = false;
    public void Init()
    {
        if (lobbyViewParent.activeSelf) lobbyViewParent.gameObject.SetActive(false);
        if (decideExitView.activeSelf) decideExitView.gameObject.SetActive(false);
        if (logOutView.activeSelf) logOutView.gameObject.SetActive(false);
        if(onceSetImage==false)
            SetImage();

        serverStateTexts[0] = "비어있음";
        serverStateTexts[1] = "원활";
        serverStateTexts[2] = "보통";
        serverStateTexts[3] = "혼잡";
        serverStateTexts[4] = "포화";
    }

    public void ActiveView() 
    {
        lobbyViewParent.SetActive(true);
        UpdateServerData();
    }

    public void UpdateServerData()
    {
        int playerCnt = SharedMgr.PhotonMgr.GetServerPlayerCnt();
        serverTexts[0].text = serverStateTexts[playerCnt];
        serverTexts[1].text = playerCnt + "/4";
        serverTexts[0].color = serverStateColors[playerCnt];
        serverTexts[1].color = serverStateColors[playerCnt];
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
        bool isExist = true;
        if(SharedMgr.PhotonMgr.IsFullRoom(ref isExist) == false)
        {
            SharedMgr.PhotonMgr.JoinLobbyRoom(isExist);
            SharedMgr.SceneMgr.LoadScene(UtilEnums.SCENES.GAME, true, true);
        }
        else
            OverFlowServer();   
    }

    public void PressGameExit()
    {
        decideExitView.SetActive(true);
    }

    public void PressLogOut()
    {
        logOutView.SetActive(true);
        SharedMgr.SaveMgr.ClearUserData();
    }

    public void DecideGameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void CancelGameExit()
    {
        decideExitView.SetActive(false);
    }

    public void DecideLogOut()
    {
        SharedMgr.SceneMgr.SetPlayerAccount("ID", string.Empty);
        SharedMgr.SceneMgr.SetPlayerAccount("PW", string.Empty);
        GetComponentInParent<LoginUICtrl>()?.ReturnLogin();
    }

    public void CancelLogOut()
    {
        logOutView.SetActive(false);
    }

    public void OverFlowServer() 
    {
        if(overFlowServerIndicator.activeSelf == false)
            overFlowServerIndicator.SetActive(true); 
    }

    public void Confirm() { overFlowServerIndicator.SetActive(false); }
    #endregion
}
