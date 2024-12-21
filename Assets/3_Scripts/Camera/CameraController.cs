using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[Header("카메라 View 종류"),SerializeField] E_CAMERAVIEW CURRENT_VIEW = E_CAMERAVIEW.CAMERA_QUATERVIEW;
    [Header("플레이어 몸통"),SerializeField] Transform playerBodyTransform;
    [SerializeField] E_CAMERAVIEW cameraView = E_CAMERAVIEW.CAMERA_CLOSEUP;
    QuaterView quaterView = null;

    private void Start()
    {
        //if (playerBodyTransform == null) playerBodyTransform = FindObjectOfType<PlayerController>().BodyTransform;
        if (cameraView == E_CAMERAVIEW.CAMERA_QUATERVIEW)
        {
            quaterView = new QuaterView(this.transform, playerBodyTransform);
            quaterView.Setup();
        }
    }

    private void Update()
    {
        if (cameraView == E_CAMERAVIEW.CAMERA_QUATERVIEW)
            quaterView.Execute();
    }

    private void LateUpdate()
    {   
        if(cameraView==E_CAMERAVIEW.CAMERA_QUATERVIEW)
            quaterView.LateExecute();
    }

    public void ChangeState()
    {
        quaterView = new QuaterView(this.transform, playerBodyTransform);
        quaterView.Setup();
        cameraView = E_CAMERAVIEW.CAMERA_QUATERVIEW;
    }

    public void QuaterViewChangeTarget(Transform _newTarget) { quaterView.ChangeTarget(_newTarget); }
}
