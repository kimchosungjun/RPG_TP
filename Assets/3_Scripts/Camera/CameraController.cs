using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[Header("카메라 View 종류"),SerializeField] E_CAMERAVIEW CURRENT_VIEW = E_CAMERAVIEW.CAMERA_QUATERVIEW;
    [Header("플레이어 몸통"),SerializeField] Transform playerBodyTransform;
    
    QuaterView quaterView = null;

    private void Start()
    { 
        //if (playerBodyTransform == null) playerBodyTransform = FindObjectOfType<PlayerController>().BodyTransform;
        quaterView = new QuaterView(this.transform, playerBodyTransform);
        quaterView.Setup();
    }

    private void Update()
    {
        quaterView.Execute();
      
    }

    private void LateUpdate()
    {
        quaterView.LateExecute();
    }
}
