using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TitleInfoView :MonoBehaviour
{
    TitleUIController uiController = null;

    [SerializeField] Image infoImage;
    [SerializeField] Text infoText;
    [SerializeField, Tooltip("페이드 시간")] float fadeTimer;
    [SerializeField, Tooltip("페이드 인 이후 페이드 아웃으로 전환하는데 기다리는 시간")] float fadeConversionTimer;
    [SerializeField, Tooltip("다음 텍스트를 출력하는데 기다리는 시간")] float waitTimer;
    
    public void Init(TitleUIController _controller) 
    {
        uiController = _controller;
      
        Color transparent_Color = Color.white;
        transparent_Color.a = 0f;
        infoImage.color = transparent_Color;
        infoText.color = transparent_Color;

        if (infoImage.gameObject.activeSelf == false) 
        {
            infoImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Test", "battle");
            infoImage.gameObject.SetActive(true);
        }
        if(infoText.gameObject.activeSelf==false) infoText.gameObject.SetActive(true);
    }

    public void FadeInfo() { StartCoroutine(CFadeInfo()); }

    IEnumerator CFadeInfo() 
    {
        Color color = infoText.color;
        float time = 0f;
        // Fade In
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time / fadeTimer);
            infoImage.color = color;
            infoText.color = color;
            yield return null;  
        }
        color.a = 1f;
        infoImage.color = color;
        infoText.color = color;

        yield return new WaitForSeconds(fadeConversionTimer);

        // Fade Out
        time = 0f;
        // Fade In
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, time / fadeTimer);
            infoImage.color = color;
            infoText.color = color;
            yield return null;
        }
        color.a = 0f;
        infoImage.color = color;
        infoText.color = color;

        yield return new WaitForSeconds(waitTimer);

        // Next Event
        if(uiController != null)
            uiController.CarryOnGameName();
    }
}
