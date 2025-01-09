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
    }


    void Start()
    {
        loginUICtrl.DoFadeIn();
        if(SharedMgr.SceneMgr.IsMaintainLogin())
        {
            SharedMgr.UIMgr.LoginUICtrl.DoOpenGate();
            loginUICtrl.DoLobby();
        }
        else
        {
            loginUICtrl.DoLogin();
        }
        // To Do ~~~~~ Load Save Data 
    }

    
}
