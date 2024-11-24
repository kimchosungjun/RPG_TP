using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RBMovement : MonoBehaviour
{
    #region Compoenent
    /*************************************************/
    /****************** 컴포넌트 *******************/
    /*************************************************/
    [Header("Component")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform cameraTransform; 
    [SerializeField, Tooltip("Have to Link")] Transform bodyTransform;
    #endregion

    #region Detect
    /*************************************************/
    /********************* 감지 *********************/
    /*************************************************/
    [Header("Common Detect")]
    [SerializeField] float playerBodyRadius;

    [Header("Ground Detect")]
    [SerializeField] float groundDetectDeltaDistance;
    [SerializeField] bool isOnGround = false; // 나중엔 지우기
    int groundLayer = 1 << 3 | 1 << 6; //3은 벽, 6은 땅
    float groundDetectDistance;
    RaycastHit groundHit;

    [Header("Slope Detect")]
    [SerializeField] float maxSlopeAngle = 60f; // 60f로 고정할 생각, 나중에 지우기
    [SerializeField] float stepHeight = 0.2f; 
    float slopeDetectDistance;
    RaycastHit slopeHit;
    #endregion

    #region Movement
    /*************************************************/
    /********************* 이동 *********************/
    /*************************************************/
    Quaternion targetRotation;
    Vector3 targetPosition = Vector3.zero;
    Vector3 movementDirection = Vector3.zero;

    float xMove;
    float yMove;
    float zMove;

    [Header("Plane Movement")]
    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float rotateSpeed = 12f;
    #endregion

    #region Gravity
    [SerializeField] bool useGravity = true;
    [SerializeField] float playerMass =1f; 
    [SerializeField] float curGravity = 0f;
    [SerializeField] float maxGravity = -100f;
    [SerializeField] float increaseGravityScale = -9.8f;
    Vector3 gravityScale = Vector3.zero;
    #endregion

    RaycastHit nextMovementHit;

    private void Awake()
    {
        if (rigid == null) rigid = GetComponentInChildren<Rigidbody>();
        if (bodyTransform == null)
        {
            Transform[] tfs = GetComponentsInChildren<Transform>();
            int cnt = tfs.Length;
            for (int i = 0; i < cnt; i++)
            {
                if (tfs[i].gameObject.name == "BodyTransform")
                {
                    bodyTransform = tfs[i];
                    break;
                }
            }
        }
    }

    private void Start()
    {
        groundDetectDistance = bodyTransform.position.y - playerBodyRadius + groundDetectDeltaDistance;
        slopeDetectDistance = stepHeight * 0.5f * 3.5f + playerBodyRadius; // 최대각도가 60도임을 가정 : 3.5f는 삼각함수 공식에 의한 비율
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // 1. 지면 확인
        DetectGround();
        // 2. 키 입력
        InputMovement();
        // 3. 카메라 방향으로 이동방향 설정
        SetCameraLookDirection();
        // 4. 경사로 계산
        SetSlopeMovement();
        // 5. 중력 계산
        SetGravity();
        // 6. 충돌체를 계산한 이동 구현
        DoMovement();
    }

    public void DetectGround()
    {
        isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down, out groundHit, groundDetectDistance, groundLayer);
    }

    public void InputMovement()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetKeyDown(KeyCode.Space) == true ? 1 : 0;
        zMove = Input.GetAxis("Vertical");
    }

    public void SetCameraLookDirection()
    {
        movementDirection = new Vector3(xMove, 0f, zMove);
        movementDirection = cameraTransform.TransformDirection(movementDirection);
        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;
    }

    public void SetSlopeMovement()
    {
        float angle = 0f ;
        if (isOnGround)
        {
            angle = Vector3.Angle(Vector3.up, groundHit.normal);
            if (angle < 0.1f)
                return;
            else if (angle < maxSlopeAngle)
            {
                movementDirection = Vector3.ProjectOnPlane(movementDirection, groundHit.normal);
                movementDirection = movementDirection.normalized;
            }
            else
            {
                if (Physics.Raycast(transform.position + Vector3.up * stepHeight * 0.5f, movementDirection, out slopeHit, slopeDetectDistance, groundLayer))
                {
                    angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    if (angle < maxSlopeAngle)
                    {
                        movementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);
                        movementDirection = movementDirection.normalized;
                    }
                    else
                    {
                        movementDirection = new Vector3(0f, movementDirection.y, 0f);
                    }
                }
            }
        }
    }

    public void SetGravity()
    {
        if (useGravity == false) return;
        if (isOnGround) return ;
        movementDirection.y = curGravity;
    }

    public void DoMovement()
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition + movementDirection * Time.deltaTime * moveSpeed;

        Vector3 capsuleStart = currentPosition + Vector3.up * playerBodyRadius;
        Vector3 capsuleEnd = currentPosition + Vector3.up * (1.8f - playerBodyRadius); // 1.8f는 캡슐 콜라이더 높이

        if (Physics.CapsuleCast(capsuleStart, capsuleEnd, playerBodyRadius, movementDirection, out RaycastHit hit,
            Vector3.Distance(currentPosition, nextPosition), groundLayer))
        {
            targetPosition = hit.point - movementDirection.normalized * playerBodyRadius;
        }
        else
        {
            targetPosition = nextPosition;
        }

        rigid.MovePosition(targetPosition);
    }

    private void FixedUpdate()
    {
        DetectGravity();
    }

    public void DetectGravity()
    {
        if (isOnGround)
            curGravity = 0f;
        else
        {
            curGravity += increaseGravityScale * Time.fixedDeltaTime * playerMass;
            if (curGravity < maxGravity)
                curGravity = maxGravity;
        }
    }



    private void OnDrawGizmos()
    {
        // 땅 체크 기즈모
        Gizmos.color = Color.red;    
        Gizmos.DrawSphere(bodyTransform.position + Vector3.down * groundDetectDistance, playerBodyRadius);
    }
}