using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeView : MonoBehaviour
{
    [Header("페이드 시간, 이미지")]
    [SerializeField, Range(0, 1f)] float fadeTime;
    [SerializeField] Image fadeImg;

    /// <summary>
    /// 밝아진다.
    /// </summary>
    public void UpdateFadeIn(UnityAction _event = null) { StartCoroutine(CFade(true,_event)); }

    /// <summary>
    /// 어두워진다.
    /// </summary>
    public void UpdateFadeOut(UnityAction _event = null) { StartCoroutine(CFade(false,_event)); }

    IEnumerator CFade(bool isFadeIn,UnityAction _event = null)
    {
        float timer = 0f;
        float start, end;

        if (isFadeIn) { start = 1f; end = 0f; }
        else { start  =0f; end = 1f; }

        Color fadeColor = fadeImg.color;
        fadeColor.a = start;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(start, end, timer/fadeTime);
            fadeImg.color = fadeColor;
            yield return null;  
        }
        fadeColor.a = end;
        fadeImg.color = fadeColor;

        if(_event != null)
            _event.Invoke();    
    }
}
