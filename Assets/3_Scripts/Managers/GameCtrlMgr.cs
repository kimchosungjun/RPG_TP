using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrlMgr : MonoBehaviour
{
    [SerializeField] PlayerCtrl playerCtrl;
    public PlayerCtrl GetPlayerCtrl { get { return playerCtrl; } }

    [SerializeField] CameraCtrl cameraCtrl;
    public CameraCtrl GetCameraCtrl { get {return cameraCtrl; } }

    private void Awake()
    {
        SharedMgr.GameCtrlMgr = this;
        LinkCtrl();
    }

    public void LinkCtrl()
    {
        if (playerCtrl == null)
            playerCtrl = FindObjectOfType<PlayerCtrl>();
        if (cameraCtrl == null)
            cameraCtrl = FindObjectOfType<CameraCtrl>();
    }
}
