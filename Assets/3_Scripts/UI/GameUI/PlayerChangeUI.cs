using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChangeUI : MonoBehaviour
{
    [SerializeField] Image[] changePictures;

    public void SetCoolTime(float _coolTime)
    {
        StartCoroutine(CChangeCoolDown(_coolTime));
    }

    IEnumerator CChangeCoolDown(float _coolTime)
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
    }
}
