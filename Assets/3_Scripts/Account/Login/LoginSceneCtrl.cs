using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneCtrl : MonoBehaviour
{
    [SerializeField] LoginUICtrl loginUICtrl;

    private void Awake()
    {
        if (loginUICtrl == null)
            loginUICtrl = FindObjectOfType<LoginUICtrl>();
        SharedMgr.SoundMgr.PlayBGM(UtilEnums.BGMCLIPS.LOGIN_BGM, false);
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
    }

    public void LoadSaveData() { SharedMgr.SaveMgr.LoadUserData(SharedMgr.UIMgr.LoginUICtrl.DoOpenGate); }
}
