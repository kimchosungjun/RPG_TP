using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginFadeInView : MonoBehaviour
{
    [SerializeField] float fadeInTimer;
    [SerializeField] Image fadeInImage;

    public void Init()
    {
        Color color = fadeInImage.color;
        color.a = 1f;
        fadeInImage.color = color;

        if (fadeInImage.gameObject.activeSelf == false) fadeInImage.gameObject.SetActive(true);
    }

    public void FadeIn(float timer = -1f)
    {
        if (timer < 0f) StartCoroutine(CFadeIn(fadeInTimer));
        else StartCoroutine(CFadeIn(timer));
    }

    IEnumerator CFadeIn(float timer) 
    {
        float time = 0f;
        Color color = fadeInImage.color;
        color.a = 1f;
        while (time < timer)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, time / timer);
            fadeInImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeInImage.color = color;
        fadeInImage.gameObject.SetActive(false);
        SharedMgr.UIMgr.LoginUICtrl.DoOpenGate();
    }
}
