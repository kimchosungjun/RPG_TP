using UnityEngine;

public class CameraQuaterView : MonoBehaviour
{
    // 벽, 땅 
    int wallNGroundLayer = 1 << (int)UtilEnums.LAYERS.WALL | 1 << (int)UtilEnums.LAYERS.GROUND;

    // 처음에 초기화해줘야 하는 값들
    Transform camTransform = null; // 카메라의 Transform
    [SerializeField] Transform lookatTransform = null; // 바라볼 목표의 Transfrom
    float deltaDistance = 5f; // 캐릭터로부터 카메라가 떨어진 거리

    // 마우스는 2D 좌표계의 개념
    float mMouseXValue = 0f; // 마우스 X값, 카메라를 Y축을 회전축으로 좌우 회전
    float mMouseYValue = 0f; // 마우스 Y값, 카메라를 X축을 회전축으로 상하 회전
    Vector3 offset = Vector3.zero;   // 캐릭터로부터 얼마나 떨어져 있는지에 대한 변위(위치)

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
        mMouseYValue = camTransform.rotation.eulerAngles.x;
        mMouseXValue = camTransform.rotation.eulerAngles.y;
        camTransform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);      // 설정된 회전값을 적용해둠
    }

    public void SetDeltaDistance(float _distance) { deltaDistance = _distance; offset = new Vector3(0f, 0f, -1f * deltaDistance); }
    #endregion

    #region Interface Camera View  

    public void Execute()
    {
        // 스크린에서는 윈도우 좌표계(2D 이용), 스크린좌표는 y축이 뒤집어져 있으므로 -1을 곱해야 한다.
        // x는 그대로, y는 반대로 
        // 마우스의 x는 회전의 y값, 마우스의 y는 회전의 x값
        float tMouseX = Input.GetAxis("Mouse X");
        float tMouseY = Input.GetAxis("Mouse Y");
        mMouseXValue += tMouseX;
        mMouseYValue += tMouseY * (-1.0f);
    }

    public void LateExecute()
    {
        if (lookatTransform == null) return;

        // 벡터끼리의 덧셈연산
        // 위치 = 위치 + 벡터
        // 위치 = 위치 + 사원수 * 벡터  <-- 사원수 * 벡터는 벡터의 결과를 내도록 유니티에 구현되어 있다.
        mMouseYValue = Mathf.Clamp(mMouseYValue, -90f, 90f);
        camTransform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);        // 오일러 각에 의한 회전을 연산하고 이것을 사원수로 변환하여 적용

        RaycastHit hit;
        Vector3 deltaVec = camTransform.rotation * offset;
        if (Physics.Raycast(lookatTransform.position, deltaVec.normalized, out hit, deltaVec.magnitude, wallNGroundLayer)) // Layer는 나중에 변경
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
