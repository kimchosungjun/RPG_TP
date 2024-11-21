using System;
using System.Collections;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform bodyTF;
    [SerializeField] float playerBodyRadius;
    [SerializeField] float groundDetectDistance;
    int groundLayer = 1 << 3 | 1 << 6;


    [SerializeField] private float speed = 5f; // 이동 속도
    [SerializeField] private float groundCheckDistance = 1.0f; // 지면 감지 Raycast 길이
    [SerializeField] private float maxSlopeAngle = 45f; // 올라갈 수 있는 최대 경사 각도

    [SerializeField] private Vector3 groundPosition; // 감지된 지면의 위치
    private Vector3 groundNormal;   // 감지된 지면의 법선

    bool isshowDebug = false;
    [SerializeField] bool isGround = false;
    [SerializeField] bool useGravity = true;
    [SerializeField] float curGravity = 0f;
    [SerializeField] float minGravity = -10f;
    [SerializeField] float maxGravity = -100f;
    [SerializeField] float increaseGravityScale = -9.8f;
    Vector3 gravityScale = Vector3.zero;

    RaycastHit groundHit;
    RaycastHit nextMovementHit;

    private void Awake()
    {
        rb =GetComponent<Rigidbody>();
        groundDetectDistance = bodyTF.position.y - playerBodyRadius + 0.1f;
    }

    void Update()
    {
        // 1. 지면 감지 및 법선 계산
        DetectGround();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;

        //moveDirection.y = Gravity();

        //RaycastHit hit;
        //Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Ray 시작 위치

        //if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer))
        //{
        //    groundPosition = hit.point;    // 감지된 지면 위치 저장
        //    groundNormal = hit.normal;     // 감지된 지면의 법선 저장
        //    isGround = true;
        //}
        //else
        //{
        //    groundPosition = transform.position; // 지면이 감지되지 않으면 현재 위치 유지
        //    groundNormal = Vector3.up;          // 평지로 간주
        //    isGround = false;
        //}



        // 2. 캐릭터 수직 위치 고정 (지면 높이에 맞춤)
        Vector3 currentPosition = transform.position;
        currentPosition.y = groundPosition.y;
        //transform.position = currentPosition;
        rb.MovePosition(currentPosition);
        // 3. 입력에 따른 수평 이동 처리
       
        // 4. 이동 방향을 슬로프에 맞게 조정
        moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal);

        // 5. 경사 이동 제한 (아래쪽 이동 허용)
        //if(isGround)
        //{
        //    float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        //    if (slopeAngle != 0f) Debug.Log(slopeAngle);
        //    if (slopeAngle > maxSlopeAngle)
        //    {
        //        // 이동 방향이 경사 아래쪽인지 확인
        //        if (Vector3.Dot(moveDirection, groundNormal) > 0)
        //        {
        //            moveDirection = Vector3.zero; // 위쪽 이동 금지
        //        }
        //    }
        //}
        

        // 6. 벽 충돌 방지
        //if (Physics.Raycast(transform.position, moveDirection, out hit, 0.5f))
        //{
        //    Vector3 wallNormal = hit.normal;
        //    moveDirection = Vector3.ProjectOnPlane(moveDirection, wallNormal); // 충돌 방향 조정
        //}

        rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    public void DetectGround()
    {
        isGround =
        Physics.SphereCast(bodyTF.position, playerBodyRadius, Vector3.down, out groundHit, groundDetectDistance, groundLayer);
    }

    public float Gravity()
    {
        if (useGravity == false) return 0f;
        if (isGround)
        {
            curGravity = minGravity;
            return 0f;
        }

        curGravity += increaseGravityScale * Time.fixedDeltaTime;
        if (curGravity < maxGravity)
            curGravity = maxGravity;

        gravityScale = Vector3.up * curGravity;
        return curGravity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;    
        Gizmos.DrawSphere(bodyTF.position + Vector3.down * groundDetectDistance, playerBodyRadius);
    }
}