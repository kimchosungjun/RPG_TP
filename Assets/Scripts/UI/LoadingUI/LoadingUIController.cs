using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUIController : MonoBehaviour
{   
    [Header("페이드, 로딩 UI")]
    [SerializeField] LoadingFrameView fadeView;
    [SerializeField] LoadingView loadingView;

    private void Awake()
    {
        if (fadeView == null) fadeView = GetComponentInChildren<LoadingFrameView>();
        if (loadingView == null) loadingView = GetComponentInChildren<LoadingView>();

        if (fadeView != null) fadeView.Init();
        if (loadingView != null) loadingView.Init();
    }

    private void Start()
    {
        fadeView.SetLoadingImage();
        UpdateLoading(true);
        SharedMgr.SceneMgr.LoadingScene(fadeView.FadeIn);
    }

    public void UpdateLoading(bool _isLoading)
    {
        if(_isLoading) loadingView.EnableLoadingView();
        else loadingView.DisableLoadingView();
    }
}
