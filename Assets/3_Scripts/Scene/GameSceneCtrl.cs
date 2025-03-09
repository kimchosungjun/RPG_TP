using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCtrl : MonoBehaviour
{
    enum IsWatchOpen
    {
        No=0,
        Yes=1,
    }

    private void Awake()
    {
        LoadUI();       
    }

    public void LoadUI()
    {
        string uiPath = "UI/" + "UIGroup";
        Instantiate(SharedMgr.ResourceMgr.LoadResource<Transform>(uiPath).gameObject);
    }

    private void Start()
    {
        SharedMgr.PhotonMgr.ManageMessageQueueRunning(true);
        StartCoroutine(COpening());
    }

    IEnumerator COpening()
    {
        yield return null;

        if(SharedMgr.SaveMgr.Option.watchOpening == (int)IsWatchOpen.No)
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true);
            SharedMgr.UIMgr.GameUICtrl.GetVideoUI.SetVideo(ReleaseMoveLock);
        }
    }

    public void ReleaseMoveLock()
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
        SharedMgr.SaveMgr.Option.watchOpening = (int)IsWatchOpen.Yes;
    }
}
