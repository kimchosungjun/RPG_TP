using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcUI : MonoBehaviour
{
    [SerializeField] GameObject saveObject;
    [SerializeField] GameObject clearObject;

    public void ShowSaveDataUI()
    {
        if (saveObject.activeSelf)
            return;
        saveObject.SetActive(true);
    }

    public void CloseShowSaveDataUI()
    {
        saveObject.SetActive(false);
    }

    public void ShowClear()
    {
        saveObject.SetActive(true);
        StartCoroutine(CShowClear());
    }

    IEnumerator CShowClear()
    {
        yield return new WaitForSeconds(5f);
        saveObject.SetActive(false);
    }
}