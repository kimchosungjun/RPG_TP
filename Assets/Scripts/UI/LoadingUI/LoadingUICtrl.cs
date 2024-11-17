using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUICtrl : MonoBehaviour
{
    [Header("UI 컨트롤러")]
    [SerializeField] GameObject ctrlObject;

    [Header("페이드, 로딩 UI")]
    [SerializeField] LoadingFrameView fadeView;
    [SerializeField] LoadingView loadingView;

    private void Awake()
    {
        if (fadeView == null) fadeView = GetComponentInChildren<LoadingFrameView>();
        if (loadingView == null) loadingView = GetComponentInChildren<LoadingView>();

        if (fadeView != null) fadeView.Init();
        if (loadingView != null) loadingView.Init();

        SharedMgr.UIMgr.LoadingUICtrl = this;
        if (ctrlObject != null) DontDestroyOnLoad(ctrlObject);
    }

    private void Start()
    {
        fadeView.SetLoadingImage();
        SharedMgr.SceneMgr.LoadingScene(UpdateAllPercent);
    }

    // 90% 전까지 
    public void UpdateLoadingPercent(float _percent) { loadingView.UpdateLoadingPercent(_percent); }
    
    // 90% 이후
    public void UpdateAllPercent() { StartCoroutine(CUpdateAllPercent()); }
    IEnumerator CUpdateAllPercent() 
    {
        while (true)
        {
            if (SharedMgr.SceneMgr.CheckEndSceneLoad())
                break;
            yield return null;
        }
        loadingView.UpdateAllPercent();
    }
    
    // 페이드 인 효과
    public void FrameFadeIn() { fadeView.FadeIn(); }
    public void DeleteCtrl() { SharedMgr.UIMgr.LoadingUICtrl = null; Destroy(ctrlObject); }
}
