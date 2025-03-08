using UnityEngine;

public partial class CameraQuaterView : MonoBehaviour
{
    #region Variable
    int wallNGroundLayer = 1 << (int)UtilEnums.LAYERS.WALL | 1 << (int)UtilEnums.LAYERS.GROUND;

    Transform camTransform = null;
    Transform lookatTransform = null;

    bool isDragging = false;
    float deltaDistance = 5f; 
    float mMouseXValue = 0f;
    float mMouseYValue = 0f;
    float touchSensitivity = 0.2f;

    Vector3 offset = Vector3.zero;   
    Vector2 lastTouchPosition;
    #endregion

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
#if UNITY_ANDROID
        MobileExecute();
#else
        PCExecute();
#endif
    }

    public void LateExecute()
    {
#if UNITY_ANDROID
        MobileLateExecute();
#else
        PCLateExecute();
#endif
    }
    #endregion
}
