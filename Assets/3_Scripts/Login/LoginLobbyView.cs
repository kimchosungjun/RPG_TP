using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginLobbyView : MonoBehaviour
{
    [SerializeField] GameObject lobbyViewParent;
    [SerializeField] GameObject decideExitView;
    [SerializeField] GameObject logOutView;
    public void Init()
    {
        if (lobbyViewParent.activeSelf) lobbyViewParent.gameObject.SetActive(false);
        if (decideExitView.activeSelf) decideExitView.gameObject.SetActive(false);
        if (logOutView.activeSelf) logOutView.gameObject.SetActive(false);
    }

    public void ActiveView() { lobbyViewParent.SetActive(true); }

    #region Btn Function
    public void PressGameStart()
    {
        SharedMgr.SceneMgr.LoadScene(E_SCENES.SCENE_GAME, true);
    }

    public void PressGameExit()
    {
        decideExitView.SetActive(true);
    }

    public void PressLogOut()
    {
        logOutView.SetActive(true);
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
        decideExitView.SetActive(false);
    }
    #endregion
}
