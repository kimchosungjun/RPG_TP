using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    //int activeCamIndex = 0;
    [SerializeField] bool isMoveLock = true;

    [SerializeField] CameraQuaterView quaterView;
    [SerializeField] CameraShakeView shakeView;
    [SerializeField] CameraTalkView talkView;
    [SerializeField] MinimapCameraView minimapView;

    private void Start()
    {
        quaterView.Setup(this.transform);
        talkView.Setup();
        shakeView.Setup();
        minimapView.Setup();
    }

    private void Update()
    {
        if (isMoveLock) return;
        quaterView.Execute();
    }

    private void LateUpdate()
    {
        minimapView.LateExecute();
        if (isMoveLock) return;
        quaterView.LateExecute();
    }

    public void ResetMoveLockView()
    {
        isMoveLock = false;
        quaterView.InitValues();
    }

    #region Set QuaterView
    public void SetQuaterView(Transform _target)
    {
        isMoveLock = false;
        quaterView.ChangeTarget(_target);
        minimapView.ChangeTarget(_target);
    }
    #endregion

    #region Set Talk View
    public void SetTalkCamerView(Transform _targetTransform)
    {
        isMoveLock = true;
        talkView.Talk(_targetTransform);
    }

    public void ReSetTalkCameraView()
    {
        talkView.EndTalk(ResetMoveLockView);
    }

    public void SetMoveRockCamera(bool _isLock)
    {
        isMoveLock = _isLock;
    }
    #endregion

    #region Set MinimapView
    public void SetMinimapTarget(Transform _target)
    {
        minimapView?.ChangeTarget(_target); 
    }

    public void Zoom(bool _isZoomIn = true)
    {
        if(_isZoomIn)
            minimapView?.ZoomIn();
        else
            minimapView?.ZoomOut();
    }
    #endregion

    #region Set Shake View
    public void SetShakeCameraView(int _id = 0)
    {
        isMoveLock = true;
        shakeView.Shake(_id);
    }
    #endregion
}
