using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 페이드, 로딩 UI 컨트롤러 : 게임 시작시에만 생성, SortOrder = 5
/// </summary>
public class CommonUIController : MonoBehaviour
{   
    [Header("페이드, 로딩 UI")]
    [SerializeField] FadeView fadeView;
    [SerializeField] LoadingView loadingView;

    private void Awake() 
    {
        // Dondestroyonload 게임 오브젝트의 자식으로 변경
        DontDestroyOnLoad(this.gameObject);
        SharedMgr.SceneMgr.CommonUIController = this;
    }

    /// <summary>
    /// True : 페이드 인(밝아짐), False : 페이드 아웃(어두워짐)
    /// </summary>
    /// <param name="_isFadeIn"></param>
    /// <param name="_event"></param>
    public void UpdateFade(bool _isFadeIn, UnityAction _event = null)
    {
        if(fadeView.gameObject.activeSelf==false) fadeView.gameObject.SetActive(true);
           
        if (_isFadeIn) fadeView.UpdateFadeIn(_event);
        else fadeView.UpdateFadeOut(_event);
    }

    public void UpdateLoading(bool _isLoading)
    {
        if(loadingView.gameObject.activeSelf==false) loadingView.gameObject.SetActive(true);

        if(_isLoading) loadingView.EnableLoadingView();
        else loadingView.DisableLoadingView();
    }
}
