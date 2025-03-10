using UnityEngine;

public class CameraQuaterView : MonoBehaviour
{
    int wallNGroundLayer = 1 << (int)UtilEnums.LAYERS.WALL | 1 << (int)UtilEnums.LAYERS.GROUND;

    Transform camTransform = null;
    Transform lookatTransform = null;
    float deltaDistance = 5f; 

    float mMouseXValue = 0f; 
    float mMouseYValue = 0f;
    Vector3 offset = Vector3.zero;   

    #region Setup & Change Information

    public void Setup(Transform _camTransform, float _deltaDistance = 7.5f)
    {
        camTransform = _camTransform;
        deltaDistance = _deltaDistance;
    }

    public void ChangeTarget(Transform _newTarget)
    {
        lookatTransform = _newTarget;
        InitValues();
    }

    public void InitValues()
    {
        offset = new Vector3(0f, 0f, -1f * deltaDistance);

        if (camTransform == null)
            camTransform = this.transform;
        mMouseYValue = camTransform.rotation.eulerAngles.x;
        mMouseXValue = camTransform.rotation.eulerAngles.y;
        camTransform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);      // 설정된 회전값을 적용해둠
    }

    public void SetDeltaDistance(float _distance) { deltaDistance = _distance; offset = new Vector3(0f, 0f, -1f * deltaDistance); }
    #endregion

    #region Interface Camera View  

    public void Execute()
    {
        float tMouseX = Input.GetAxis("Mouse X");
        float tMouseY = Input.GetAxis("Mouse Y");
        mMouseXValue += tMouseX;
        mMouseYValue += tMouseY * (-1.0f);
    }

    public void LateExecute()
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
