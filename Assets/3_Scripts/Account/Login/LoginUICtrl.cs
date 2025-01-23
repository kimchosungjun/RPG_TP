using UnityEngine;

public class LoginUICtrl : MonoBehaviour
{
    // preload 기능 추가
    [Header("씬의 Flow를 관리하는 씬 컨트롤러")]
    [SerializeField] LoginSceneCtrl sceneCtrl;

    public LoginSceneCtrl GetSceneCtrl { get { return sceneCtrl; } } 

    [Header("UI의 정보를 표기하는 View")]
    [SerializeField] LoginLobbyView lobbyView;
    [SerializeField] LoginGateView gateView;    
    [SerializeField] LoginInputView inputView;
    [SerializeField] LoginFadeInView fadeInView;

    public LoginInputView GetLoginInputView { get { return inputView; }  }
    public LoginGateView GetLoginGateView { get { return gateView; } }
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
    public void ReturnLogin() { lobbyView.Init(); inputView.Init(); gateView.CloseDoor(); }
}
