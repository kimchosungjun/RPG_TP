using System;
using System.Collections;
using UnityEngine;

public partial class CharacterCtrl : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] CapsuleCollider coll;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Transform mainCamTransform;

    [Header("이동")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float limitMoveSpeed;
    [SerializeField] float airDrag =0.1f;
    [SerializeField] float groundDrag =2f;
    
    Vector3 moveDirection;
    Quaternion targetRotation;
    float moveVertical;
    float moveHorizontal;

    [Header("검출")]
    [SerializeField] bool isGround = false;
    [SerializeField] bool isOnSlope = false;
    [SerializeField] float maxSlopeAngle = 45f;
    [SerializeField] float detectRadiusDeltaDistance = 0.1f;
    [SerializeField] float detectGroundDeltaDistance = 0.1f;
    //[SerializeField] float stepDeltaDistance = 0.2f;

    RaycastHit groundHit;
    RaycastHit slopeHit;
    int groundLayer = 1 << 3 | 1 << 6;
    float groundDetectDistance;
    float playerBodyRadius;

    [Header("중력")]
    [SerializeField] float curGravityScale = -100.0f;
    [SerializeField] float minGravityScale = -100.0f;
    [SerializeField] float maxGravityScale = -500.0f;
    [SerializeField] float gravityIncreaseTime = 0.0f;
    [SerializeField] float gravityIncreaseTimer = 0.05f;
    [SerializeField, Range(0.0f, -50.0f)] float gravityIncreasement = -9.8f;

    Vector3 gravityScale;

    /// <summary>
    /// Call By Start : 초기화 할 변수 모음
    /// </summary>
    public void MovementSetup()
    {
        if (mainCamTransform == null) mainCamTransform = Camera.main.transform;
        if (coll == null) coll = GetComponent<CapsuleCollider>();

        targetRotation = transform.rotation;
        playerBodyRadius = coll.radius + detectRadiusDeltaDistance;
        groundDetectDistance = bodyTransform.position.y + detectGroundDeltaDistance;
    }

    public void MovementInput()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");
    }

    public void SetMoveDirection() 
    {
        moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        moveDirection = mainCamTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;
    }

    public void CheckGround()
    {
        isGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down,
            out groundHit, groundDetectDistance, groundLayer);

        if (isGround) rb.drag = groundDrag;
        else rb.drag = airDrag;
    }

    public void SetSlopeMovement()
    {
        if (isGround)
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            // 0도라면, 평면 위
            if (angle == 0f)
                return;

            // 최대각도보다 아래라면 해당 각도에 맞는 방향으로 힘 가하기
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundHit.normal);
            moveDirection = moveDirection.normalized;
            // 최대각도보다 크다면 아래로 힘 가하기
            if (angle > maxSlopeAngle)
                moveDirection.y = -0.1f * angle;
        }
    }
  
    public void SetMoveRotation()
    {
        Quaternion curRotation = transform.rotation;
        if (moveDirection == Vector3.zero)
        {
            if(Quaternion.Angle(targetRotation, curRotation)<3f)
                return;
            transform.rotation = Quaternion.Slerp(curRotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        }
        else
        {
            Vector3 lookDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
            if (lookDirection == Vector3.zero) return;

            lookDirection = mainCamTransform.TransformDirection(lookDirection);
            lookDirection.y = 0f;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            targetRotation = lookRotation;
            transform.rotation = Quaternion.Slerp(curRotation, lookRotation, Time.fixedDeltaTime * rotateSpeed);
        }
    }



    public void LimitSpeed()
    {
        Vector3 velocityVec = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        if (isOnSlope && isGround)
        {
            if (velocityVec.magnitude > limitMoveSpeed)
            {
                velocityVec = velocityVec.normalized;
                rb.velocity = velocityVec * limitMoveSpeed;
            }
            return;
        }

        velocityVec.y = 0f;
        velocityVec = velocityVec.normalized;

        rb.velocity = new Vector3(velocityVec.x * limitMoveSpeed, rb.velocity.y, velocityVec.z * limitMoveSpeed);
    }

    public void SetGravity()
    {
        if (isGround)
        {
            curGravityScale = minGravityScale;
            gravityScale = Vector3.zero;
            return;
        }
        gravityIncreaseTime += Time.fixedDeltaTime;
        if (gravityIncreaseTime >= gravityIncreaseTimer)
        {
            gravityIncreaseTime = 0f;
            curGravityScale += gravityIncreasement * Time.fixedDeltaTime;
            if (curGravityScale < maxGravityScale)
                curGravityScale = maxGravityScale;
        }
        gravityScale = Vector3.up * curGravityScale;
    }
}
