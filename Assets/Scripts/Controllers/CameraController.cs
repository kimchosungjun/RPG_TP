using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라가 바라보는 오브젝트
    [SerializeField]
    GameObject mLookAtObject;

    // 캐릭터로부터 카메라가 떨어진 거리
    [SerializeField]
    float mArmLength;

    // 마우스는 2D 좌표계의 개념
    float mMouseXValue = 0f; // 마우스 X값, 카메라를 Y축을 회전축으로 좌우 회전
    float mMouseYValue = 0f; // 마우스 Y값, 카메라를 X축을 회전축으로 상하 회전

    // 캐릭터로부터 얼마나 떨어져 있는지에 대한 변위(위치)
    [SerializeField] Vector3 mOffset = Vector3.zero;

    private void Start()
    {
        mOffset = new Vector3(0f, 0f, -1f * mArmLength);
        mMouseYValue = this.transform.rotation.eulerAngles.x;
        mMouseXValue = this.transform.rotation.eulerAngles.y;

        // 설정된 회전값을 적용해둠
        this.transform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);
    }

    private void Update()
    {
        float tMouseX = Input.GetAxis("Mouse X");
        float tMouseY = Input.GetAxis("Mouse Y");

        // 스크린에서는 윈도우 좌표계(2D 이용), 스크린좌표는 y축이 뒤집어져 있으므로 -1을 곱해야 한다.
        // x는 그대로, y는 반대로 
        // 마우스의 x는 회전의 y값, 마우스의 y는 회전의 x값
        mMouseXValue += tMouseX;
        mMouseYValue += tMouseY * (-1.0f);


        if (mMouseYValue > 90f)
            mMouseYValue = Mathf.Clamp(mMouseYValue, -90f, 90f);
        else if(mMouseYValue<0f)
            mMouseYValue = Mathf.Clamp(mMouseYValue, -90f, 90f);

        // 오일러 각에 의한 회전을 연산하고 이것을 사원수로 변환하여 적용
        this.transform.rotation = Quaternion.Euler(mMouseYValue, mMouseXValue, 0f);

    }

    private void LateUpdate()
    {
        // 벡터끼리의 덧셈연산
        // 위치 = 위치 + 벡터
        //this.transform.position = mLookAtObject.transform.position + mOffset;

        // 위치 = 위치 + 사원수 * 벡터
        // <-- 사원수 * 벡터는 벡터의 결과를 내도록 유니티에 구현되어 있다.

        RaycastHit hit;
        Vector3 deltaVec = this.transform.rotation * mOffset;
        if(Physics.Raycast(mLookAtObject.transform.position, deltaVec.normalized, out hit, deltaVec.magnitude, LayerMask.GetMask("Wall")))
        {
            float distance = (hit.point - mLookAtObject.transform.position).magnitude * 0.8f;
            this.transform.position = mLookAtObject.transform.position + distance * deltaVec.normalized;
        }
        else
        {
            this.transform.position = mLookAtObject.transform.position + this.transform.rotation * mOffset;
        }
    }


    //private void LateUpdate()
    //{
    //    RaycastHit hit;
    //    if(Physics.Raycast(playerTransform.position,deltaVector,out hit, deltaVector.magnitude, LayerMask.GetMask("Wall")))
    //    {
    //        float dist = (hit.point - playerTransform.position).magnitude * 0.8f;
    //        transform.position = playerTransform.position + deltaVector.normalized * dist;
    //    }
    //    else
    //    {
    //        transform.position = playerTransform.position + deltaVector;
    //        transform.LookAt(playerTransform.position);
    //    }
    //}


}
