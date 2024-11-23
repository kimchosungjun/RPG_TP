using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBForceMovement : MonoBehaviour
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
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float maxSlopeAngle = 60f; // 60f로 고정할 생각, 나중에 지우기
    float limitSlopeMovementAngle = 80f;
    float slopeDetectDistance;
    RaycastHit slopeHit;
    #endregion

    #region Movement
    /*************************************************/
    /********************* 이동 *********************/
    /*************************************************/
    Quaternion targetRotation;
    Vector3 movementDirection = Vector3.zero;

    float xMove;
    float yMove;
    float zMove;

    [Header("Plane Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 12f;
    [SerializeField] float limitMoveSpeed = 7f;
    #endregion

    #region Gravity
    [SerializeField] bool useGravity = true;
    [SerializeField] float playerMass = 1f;
    [SerializeField] float curGravity = 0f;
    [SerializeField] float maxGravity = -100f;
    [SerializeField] float increaseGravityScale = -9.8f;
    Vector3 gravityScale = Vector3.zero;
    #endregion

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
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        InputMovement();
    }

    public void InputMovement()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetKeyDown(KeyCode.Space) == true ? 1 : 0;
        zMove = Input.GetAxis("Vertical");
    }


    private void FixedUpdate()
    {
        // 1. 땅 체크
        CheckGround();
        // 2. 이동 방향 설정
        SetCameraLookDirection();
        // 3. 경사로 처리
        SetSlopeMovement();
        // 4. 이동 구현
        ApplyMovementForce();
        // 5. 중력 적용
        ApplyGravity();
        // 6. 회전 설정
        SetRotation();
        // 7. 속도 제한
        LimitSpeed();
    }

    public void CheckGround() { isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down, out groundHit, groundDetectDistance, groundLayer); }

    public void SetCameraLookDirection()
    {
        movementDirection = new Vector3(xMove, 0f, zMove);
        movementDirection = cameraTransform.TransformDirection(movementDirection);
        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;
    }

    private void ApplyGravity()
    {
        if (!useGravity) return;

        if (isOnGround)
        {
            curGravity = 0f; // 땅에 닿으면 중력을 초기화
            return;
        }

        curGravity += increaseGravityScale * Time.fixedDeltaTime * playerMass;
        if (curGravity < maxGravity)
            curGravity = maxGravity;

        if (IsTouchingWall(out Vector3 wallNormal))
        {
            // 벽에서 미끄러짐
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, wallNormal).normalized;
            rigid.AddForce(slideDirection * Mathf.Abs(curGravity), ForceMode.Acceleration);
        }
        else
        {
            // 공중 중력
            rigid.AddForce(Vector3.up * curGravity, ForceMode.Acceleration);
        }
    }

    private void SetSlopeMovement()
    {
        if (isOnGround)
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);

            if (angle < maxSlopeAngle)
            {
                // 경사면에서 이동 방향을 평면화
                movementDirection = Vector3.ProjectOnPlane(movementDirection, groundHit.normal).normalized;
            }
            else
            {
                // 경사면이 너무 가파르면 이동 방향을 제한
                movementDirection = Vector3.zero;
            }
        }
    }

    private bool IsTouchingWall(out Vector3 wallNormal)
    {
        // 벽 감지
        float wallDetectRadius = playerBodyRadius * 1.1f;
        float wallDetectDistance = playerBodyRadius * 0.5f;

        if (Physics.SphereCast(transform.position, wallDetectRadius, movementDirection, out RaycastHit wallHit, wallDetectDistance, groundLayer))
        {
            wallNormal = wallHit.normal;
            return true;
        }

        wallNormal = Vector3.zero;
        return false;
    }

    public void SetRotation()
    {
        Vector3 inputDirection = new Vector3(xMove, 0, zMove);
        if (inputDirection != Vector3.zero)
        {
            inputDirection = cameraTransform.TransformDirection(inputDirection);
            inputDirection.y = 0f;
            inputDirection = inputDirection.normalized;
            targetRotation = Quaternion.LookRotation(inputDirection);
        }

        if (Quaternion.Angle(rigid.rotation, targetRotation) < 1f) return;
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed));
    }

    public void LimitSpeed()
    {
        if (isOnGround)
        {
            Vector3 groundVelocity = rigid.velocity;
            groundVelocity= groundVelocity.normalized;
            if (rigid.velocity.magnitude > limitMoveSpeed)
            {
                rigid.velocity = groundVelocity * limitMoveSpeed;
            }
        }
        else
        {
            Vector3 airVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            if (airVelocity.magnitude > limitMoveSpeed)
            {
                airVelocity = airVelocity.normalized * limitMoveSpeed;
                airVelocity.y = rigid.velocity.y;
                rigid.velocity = airVelocity;
            }
        }
        
    }

    private void ApplyMovementForce()
    {
        // 이동 힘을 추가
        rigid.AddForce(movementDirection * moveSpeed, ForceMode.VelocityChange);
    }
}


