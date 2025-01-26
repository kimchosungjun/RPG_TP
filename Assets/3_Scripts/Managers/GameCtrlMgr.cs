using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrlMgr : MonoBehaviour
{
    #region Player
    [SerializeField] PlayerCtrl playerCtrl;
    public PlayerCtrl GetPlayerCtrl { get { return playerCtrl; } }

    [SerializeField] PlayerStatCtrl playerStatCtrl;
    public PlayerStatCtrl GetPlayerStatCtrl { get { return playerStatCtrl; } }
    #endregion

    #region Camera
    [SerializeField] CameraCtrl cameraCtrl;
    public CameraCtrl GetCameraCtrl { get {return cameraCtrl; } }
    #endregion

    #region Zone
    [SerializeField ]ZoneCtrl zoneCtrl;
    public ZoneCtrl GetZoneCtrl { get { return zoneCtrl; } }
    #endregion

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
        if(playerStatCtrl==null)
            playerStatCtrl = GetComponent<PlayerStatCtrl>();
        if(zoneCtrl == null)
            zoneCtrl = FindObjectOfType<ZoneCtrl>();    
    }
}
