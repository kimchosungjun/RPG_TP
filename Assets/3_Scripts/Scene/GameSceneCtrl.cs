using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCtrl : MonoBehaviour
{
    private void Awake()
    {
        LoadUI();       
    }

    public void LoadUI()
    {
        string uiPath = "UI/" + "UIGroup";
        Instantiate(SharedMgr.ResourceMgr.LoadResource<Transform>(uiPath).gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SharedMgr.ResourceMgr.PhotonRoomTestInstantiate();
        }
    }
}
