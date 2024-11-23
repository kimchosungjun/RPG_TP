using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFrameView : MonoBehaviour
{
    [SerializeField] float fadeTimer;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image loadingImage;

    public void Init()
    {
        if(loadingImage==null)
            loadingImage = GetComponentInChildren<Image>(); 
        if(canvasGroup==null)
            canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void SetLoadingImage() 
    {
        // To Do ~~ 
        // 게임 속 플레이 이미지 삽입 예정
    }

    public void FadeIn() { StartCoroutine(CFadeIn()); }

    IEnumerator CFadeIn()
    {
        canvasGroup.alpha = 1f;
        float time = 0f;
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f,0f,time/fadeTimer);   
            yield return null;
        }
        canvasGroup.alpha = 0f;

        SharedMgr.UIMgr.LoadingUICtrl.DeleteCtrl();
    }
}
