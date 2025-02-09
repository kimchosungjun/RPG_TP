using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraView : MonoBehaviour
{
    Transform miniTransform = null;
    Transform target = null;
    int curZoom = 0;
    int minZoom = -3;
    int maxZoom = 3;
    Vector3 offSet = Vector3.zero;
    Vector3 zoomOffSet = new Vector3(0, 2.5f, 0);
    public void Setup()
    {
        miniTransform = this.transform;
        offSet = new Vector3(0, 20f, 0);
    }

    public void LateExecute()
    {
        if (target == null)
            return;

        miniTransform.position = target.position + offSet;
    }

    public void ChangeTarget(Transform _target)
    {
        target = _target;   
    }

    public void ZoomIn()
    {
        curZoom -= 1;
        if (curZoom < minZoom)
        {
            curZoom = minZoom;
            return;
        }
        offSet += zoomOffSet * -1;
    }

    public void ZoomOut() 
    {
        curZoom += 1;
        if (curZoom > maxZoom)
        {
            curZoom=maxZoom;
            return ;
        }
        offSet += zoomOffSet * 1;
    }
}
