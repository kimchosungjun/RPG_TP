using System.Collections;
using System.Collections.Generic;
using UIEnums;
using UnityEngine;

public class UIModelCam : MonoBehaviour
{
    int curID = -1;
    [SerializeField] CamModel[] models;
    [SerializeField] GameObject parent;

    private void Awake()
    {
        int cnt = models.Length;
        for(int i=0; i < cnt; i++)
        {
            models[i].InActive();
        }
    }

    public void ChangeModel(int _id)
    {
        if (curID == _id) return;
        if(curID !=-1)
            models[curID].InActive();
        models[_id].Active();
        curID = _id;
    }

    public void TurnOn()
    {
        if(parent.activeSelf==false)
            parent.SetActive(true);
    }

    public void TurnOff()
    {
        if (parent.activeSelf )
            parent.SetActive(false);
    }
}
