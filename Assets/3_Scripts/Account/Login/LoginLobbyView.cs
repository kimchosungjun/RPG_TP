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

    bool onceSetImage = false;
    public void Init()
    {
        if (lobbyViewParent.activeSelf) lobbyViewParent.gameObject.SetActive(false);
        if (decideExitView.activeSelf) decideExitView.gameObject.SetActive(false);
        if (logOutView.activeSelf) logOutView.gameObject.SetActive(false);
        if(onceSetImage==false)
            SetImage();
    }

    public void ActiveView() { lobbyViewParent.SetActive(true); }

    public void SetImage()
    {
        onceSetImage = true;
        Sprite buttonSprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Red_Frame");
        buttonImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "StartGame_Frame"); 
        buttonImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Door_Icon"); 
        buttonImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon"); 
        exitImages[0].sprite = buttonSprite;
        exitImages[1].sprite = buttonSprite;
        logoutImages[0].sprite = buttonSprite;
        logoutImages[1].sprite = buttonSprite;
    }

    #region Btn Function
    public void PressGameStart()
    {
        SharedMgr.SceneMgr.LoadScene(SCENES.GAME, true);
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
    #endregion
}
