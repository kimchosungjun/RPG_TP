using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public CameraCtrl GetCameraCtrl { get { return cameraCtrl; } }
    #endregion

    #region Zone
    [SerializeField] ZoneCtrl zoneCtrl;
    public ZoneCtrl GetZoneCtrl { get { return zoneCtrl; } }
    #endregion

    #region Post Processing
    [SerializeField] Volume postProcess;
    MotionBlur motionBlur = null;
    public void OnMontionBlur()
    {
        if (motionBlur != null)
        {
            motionBlur.active = true;
            return;
        }

        if (postProcess.profile.TryGet<MotionBlur>(out motionBlur))
        {
            motionBlur.active= true;
        }
    }

    public void OffMotionBlur()
    {
        if (motionBlur != null)
        {
            motionBlur.active = false;
            return;
        }

        if (postProcess.profile.TryGet<MotionBlur>(out motionBlur))
        {
            motionBlur.active = false;
        }
    }
    #endregion

    #region Awake
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
    #endregion
}
