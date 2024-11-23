using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class TitleGameNameView :MonoBehaviour
{
    TitleUIController uiController = null;

    [SerializeField] Text gameTitle_KoreanText;
    [SerializeField] Text gameTitle_EnglishText;
    [SerializeField, Tooltip("페이드 시간")] float fadeTimer;
    [SerializeField, Tooltip("페이드 인 이후 페이드 아웃으로 전환하는데 기다리는 시간")] float fadeConversionTimer;
    [SerializeField, Tooltip("다음 텍스트를 출력하는데 기다리는 시간")] float waitTimer;
    
    public void Init(TitleUIController _controller)
    {
        uiController = _controller;

        Color transparent_Color = Color.white;
        transparent_Color.a = 0f;
        gameTitle_KoreanText.color = transparent_Color;
        gameTitle_EnglishText.color = transparent_Color;

        if (gameTitle_KoreanText.gameObject.activeSelf == false) gameTitle_KoreanText.gameObject.SetActive(true);
        if (gameTitle_EnglishText.gameObject.activeSelf == false) gameTitle_EnglishText.gameObject.SetActive(true);
    }

    public void FadeInfo(UnityAction action = null) { StartCoroutine(CFadeInfo(action)); }

    IEnumerator CFadeInfo(UnityAction action = null) 
    {
        Color color = gameTitle_EnglishText.color;
        float time = 0f;
        // Fade In
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time / fadeTimer);
            gameTitle_KoreanText.color = color;
            gameTitle_EnglishText.color = color;
            yield return null;  
        }
        color.a = 1f;
        gameTitle_KoreanText.color = color;
        gameTitle_EnglishText.color = color;

        yield return new WaitForSeconds(fadeConversionTimer);

        // Fade Out
        time = 0f;
        // Fade In
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, time / fadeTimer);
            gameTitle_KoreanText.color = color;
            gameTitle_EnglishText.color = color;
            yield return null;
        }
        color.a = 0f;
        gameTitle_KoreanText.color = color;
        gameTitle_EnglishText.color = color;

        yield return new WaitForSeconds(waitTimer);

        // Next Event
        if (uiController != null)
            uiController.CarryOnNextGameScene();
    }
}
