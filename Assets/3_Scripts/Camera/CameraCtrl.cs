using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    int activeCamIndex = 0;
    [SerializeField] bool isMoveLock = true;

    CameraQuaterView quaterView = null;
    [SerializeField] CameraShakeView shakeView;
    [SerializeField] CameraTalkView talkView;
    [SerializeField] MainCamera mainCam;

    public MainCamera GetMainCam { get { return mainCam; } }

    private void Start()
    {
        quaterView = new CameraQuaterView(this.transform);
        talkView.Setup();
        shakeView.Setup();
    }

    private void Update()
    {
        if (isMoveLock) return;
        quaterView.Execute();
    }

    private void LateUpdate()
    {
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
    }
    #endregion

    #region Set Shake View
    public void SetShakeCameraView(int _id = 0)
    {
        isMoveLock = true;
        shakeView.Shake(_id);
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
    #endregion
}
