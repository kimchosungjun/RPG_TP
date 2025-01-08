using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoginUICtrl : MonoBehaviour
{
    // preload 기능 추가
    [Header("씬의 Flow를 관리하는 씬 컨트롤러")]
    [SerializeField] LoginSceneCtrl sceneCtrl;

    [Header("UI의 정보를 표기하는 View")]
    [SerializeField] LoginInputView inputView;
    [SerializeField] LoginLobbyView lobbyView;
    [SerializeField] LoginGateView gateView;    
    [SerializeField] LoginFadeInView fadeInView;

    private void Awake()
    {
        inputView.Init();
        lobbyView.Init();
        gateView.Init();
        fadeInView.Init();
        if (sceneCtrl == null) sceneCtrl = FindObjectOfType<LoginSceneCtrl>();
        SharedMgr.UIMgr.LoginUICtrl = this;
    }

    public void DoOpenGate() { gateView.OpenGate(); }
    public void DoLogin() { inputView.ActiveView(); }
    public void DoFadeIn() { fadeInView.FadeIn(); }
    public void DoLobby() { lobbyView.ActiveView(); }
    public void ReturnLogin() { lobbyView.Init(); inputView.Init(); inputView.ActiveView(); }
}
