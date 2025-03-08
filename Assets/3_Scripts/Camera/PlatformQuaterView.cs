using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraQuaterView : MonoBehaviour
{
    #region Mobile
    public void MobileExecute()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;

                mMouseXValue += delta.x * touchSensitivity;
                mMouseYValue -= delta.y * touchSensitivity;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    public void MobileLateExecute()
    {
        if (lookatTransform == null) return;

        mMouseYValue = Mathf.Clamp(mMouseYValue, -90f, 90f);
        camTransform.rotation = Quaternion.Euler(mMouseYValue, mMouseYValue, 0f);

        RaycastHit hit;
        Vector3 deltaVec = camTransform.rotation * offset;
        if (Physics.Raycast(lookatTransform.position, deltaVec.normalized, out hit, deltaVec.magnitude, wallNGroundLayer))
        {
            float distance = (hit.point - lookatTransform.position).magnitude * 0.9f;
            camTransform.position = lookatTransform.position + distance * deltaVec.normalized;
        }
        else
        {
            camTransform.position = lookatTransform.position + camTransform.rotation * offset;
        }
    }
    #endregion

    #region PC
    public void PCExecute()
    {
        float tMouseX = Input.GetAxis("Mouse X");
        float tMouseY = Input.GetAxis("Mouse Y");
        mMouseXValue += tMouseX;
        mMouseYValue += tMouseY * (-1.0f);
    }

    public void PCLateExecute()
    {
        if (lookatTransform == null) return;

        mMouseYValue = Mathf.Clamp(mMouseYValue, -90f, 90f);
        camTransform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);     

        RaycastHit hit;
        Vector3 deltaVec = camTransform.rotation * offset;
        if (Physics.Raycast(lookatTransform.position, deltaVec.normalized, out hit, deltaVec.magnitude, wallNGroundLayer))
        {
            float distance = (hit.point - lookatTransform.position).magnitude * 0.9f;
            camTransform.position = lookatTransform.position + distance * deltaVec.normalized;
        }
        else
        {
            camTransform.position = lookatTransform.position + camTransform.rotation * offset;
        }
    }
    #endregion
}
