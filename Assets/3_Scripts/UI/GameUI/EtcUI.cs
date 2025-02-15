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
        clearObject.SetActive(true);
        Invoke("CloseClear",5f);
    }

    public void CloseClear()
    {
        clearObject?.SetActive(false);
    }
}