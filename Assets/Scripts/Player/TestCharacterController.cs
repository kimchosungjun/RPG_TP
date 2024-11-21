using System;
using System.Collections;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // 이동 속도
    [SerializeField] private float groundCheckDistance = 1.0f; // 지면 감지 Raycast 길이
    [SerializeField] private LayerMask groundLayer; // 지면 레이어
    [SerializeField] private float maxSlopeAngle = 45f; // 올라갈 수 있는 최대 경사 각도

    private Vector3 groundPosition; // 감지된 지면의 위치
    private Vector3 groundNormal;   // 감지된 지면의 법선

    void Update()
    {
        // 1. 지면 감지 및 법선 계산
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Ray 시작 위치

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            groundPosition = hit.point;    // 감지된 지면 위치 저장
            groundNormal = hit.normal;     // 감지된 지면의 법선 저장
        }
        else
        {
            groundPosition = transform.position; // 지면이 감지되지 않으면 현재 위치 유지
            groundNormal = Vector3.up;          // 평지로 간주
        }

        // 2. 캐릭터 수직 위치 고정 (지면 높이에 맞춤)
        Vector3 currentPosition = transform.position;
        currentPosition.y = groundPosition.y;
        transform.position = currentPosition;

        // 3. 입력에 따른 수평 이동 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        // 4. 이동 방향을 슬로프에 맞게 조정
        moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal);

        // 5. 경사 이동 제한 (아래쪽 이동 허용)
        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        if (slopeAngle > maxSlopeAngle)
        {
            // 이동 방향이 경사 아래쪽인지 확인
            if (Vector3.Dot(moveDirection, groundNormal) > 0)
            {
                moveDirection = Vector3.zero; // 위쪽 이동 금지
            }
        }

        // 6. 벽 충돌 방지
        if (Physics.Raycast(transform.position, moveDirection, out hit, 0.5f))
        {
            Vector3 wallNormal = hit.normal;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, wallNormal); // 충돌 방향 조정
        }

        // 7. 최종 이동 처리
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}