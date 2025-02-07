using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneCtrl : MonoBehaviour
{
    [SerializeField] LoginUICtrl loginUICtrl;

    bool isActiveExitWindow = false;
    public bool SetActiveEditWindow { set { isActiveExitWindow = value; } }
    private void Awake()
    {
        if (loginUICtrl == null)
            loginUICtrl = FindObjectOfType<LoginUICtrl>();
    }


    void Start()
    {
        loginUICtrl.DoFadeIn();
        if(SharedMgr.SceneMgr.IsMaintainLogin())
        {
            LoadSaveData();
            loginUICtrl.DoLobby();
        }
        else
        {
            loginUICtrl.DoLogin();
        }
        SharedMgr.SoundMgr.PlayBGM(UtilEnums.BGMCLIPS.LOGIN_BGM, false);
    }

    public void LoadSaveData() { SharedMgr.SaveMgr.LoadUserData(SharedMgr.UIMgr.LoginUICtrl.DoOpenGate); }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        InputExitKey();
#endif
    }

    public void InputExitKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isActiveExitWindow)
                SharedMgr.UIMgr.LoginUICtrl.GetLoginLobbyView.CancelGameExit();
            else
                SharedMgr.UIMgr.LoginUICtrl.GetLoginLobbyView.PressGameExit();
        }
    }
}
