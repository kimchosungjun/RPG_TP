using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcUI : MonoBehaviour
{
    [SerializeField] GameObject saveObject;

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
}