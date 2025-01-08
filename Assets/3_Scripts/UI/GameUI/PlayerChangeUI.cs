using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerChangeUI : MonoBehaviour
{
    [SerializeField] Image[] changePictures;
    [SerializeField] GameObject uiFrameParent;

    public void SetCoolTime(float _coolTime, UnityAction _announceCoolDown)
    {
        StartCoroutine(CChangeCoolDown(_coolTime, _announceCoolDown));
    }

    IEnumerator CChangeCoolDown(float _coolTime, UnityAction _announceCoolDown)
    {
        float time = 0f;
        int cnt = changePictures.Length;
        for (int i = 0; i < cnt; i++)
        {
            changePictures[i].fillAmount = 0f;
        }
        while (time <_coolTime)
        {
            time += Time.deltaTime; 
            for (int i = 0; i < cnt; i++)
            {
                changePictures[i].fillAmount = Mathf.Lerp(0, 1, time / _coolTime);
                yield return null;
            }
        }
        for (int i = 0; i < cnt; i++)
        {
            changePictures[i].fillAmount = 1;
        }

        if (_announceCoolDown != null)
            _announceCoolDown.Invoke();
    }

    public void Active()
    {
        uiFrameParent.SetActive(true);
    }

    public void InActive()
    {
        uiFrameParent.SetActive(false);
    }
}
