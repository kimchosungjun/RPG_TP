using System;
using System.Collections;
using UnityEngine;

public partial class CharacterCtrl : MonoBehaviour
{
    [SerializeField] int tempCombo;
    #region Component

    [Header("컴포넌트 : 연결해주기")]
    [SerializeField] Transform camTransform;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    [SerializeField] CapsuleCollider coll;
    #endregion

    #region Movement

    /******************************************/
    /************** 평면 이동 ****************/
    /******************************************/
    [Header("이동")]
    [SerializeField] float playerMoveLimitSpeed  =5f;
    [SerializeField] float playerWalkSpeed = 5f;
    [SerializeField] float playerRotateSpeed = 12f;
    [SerializeField] float planeCoefficient = 10f;
    [SerializeField] float slopeCoefficient = 20f;

    float xMove;
    float zMove;
    float playerMoveSpeed;
    Vector3 moveDirection;
    Quaternion moveRotation;

    /******************************************/
    /***************** 점프  ******************/
    /******************************************/
    [SerializeField] float playerJumpForce;
    [SerializeField, Range(0, 1f)] float airMovementMultiplier;

    /******************************************/
    /***************** 대쉬  ******************/
    /******************************************/
    [SerializeField] float playerSprintSpeed;
    [SerializeField] float sprintMaintainTimer = 0.15f;
    [SerializeField] float sprintCoolTimer = 0.3f;
    bool isPlayerSprint = false;
    bool canSprint = true;
    #endregion

    #region Detect

    /******************************************/
    /*********** 공통 검출 변수 *************/
    /******************************************/
    int groundLayer = 1 << 3 | 1 << 6; // 3은 벽, 6은 땅
    float playerBodyRadius;
    [SerializeField] float playerBodyHeight;
    /******************************************/
    /**************** 땅 검출 ****************/
    /******************************************/
    [Header("검출")]
    [SerializeField, Tooltip("땅 검출 시, 추가 검출 거리")] float detectGroundDelta = 0.1f;
    [SerializeField, Tooltip("땅 저항 값")] float groundDrag = 6;
    [SerializeField, Tooltip("공기 저항 값")] float airDrag = 1;
    bool isOnGround = false;
    float groundDetectDistance;
    RaycastHit groundHit;

    /******************************************/
    /************ 경사로 검출 ***************/
    /******************************************/
    [SerializeField] float slopeMaxAngle = 50f;
    [SerializeField] float stepHeight = 0.2f;
    bool isOnMaxAngleSlope = false;
    float slopeDetectDistance;
    RaycastHit slopeHit;

    /******************************************/
    /************** 벽 검출 ******************/
    /******************************************/
    [SerializeField] float wallCheckDistanceDelta;
    #endregion

    #region Gravity
    /******************************************/
    /**************** 중력   ******************/
    /******************************************/
    [Header("중력")]
    [SerializeField] bool useGravity = true;
    [SerializeField] float gravityIncreasemenet = -9.8f;
    [SerializeField] float curGravity = 0f;
    [SerializeField] float minGravity = 0f;
    [SerializeField] float maxGravity =0f;
    #endregion

    public void UpdateAnimation()
    {
        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        float magnitude = planeVelocity.magnitude;
        if(isOnMaxAngleSlope) anim.SetFloat("PlaneVelocity", 0f);
        else anim.SetFloat("PlaneVelocity", Mathf.Clamp(magnitude, 0f, 5f));
    }

    public void GroundCheck()
    {
        isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down, out groundHit, groundDetectDistance, groundLayer);
        if (isOnGround)
            rigid.drag = groundDrag;
        else
            rigid.drag = airDrag;

        anim.SetBool("IsOnGround", isOnGround);
    }

    public void InputMovementKey()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !isOnMaxAngleSlope) { Jump(); }
        if (Input.GetMouseButtonDown(1)) { Sprint(); }
    }

    public void Jump()
    {
        anim.SetTrigger("Jump");
        anim.SetBool("IsOnGround", false);
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    }

    public void Sprint()
    {
        if (canSprint)
        {
            canSprint = false;
            playerMoveSpeed = playerSprintSpeed;
            StartCoroutine(CSprintCoolState());
        }
    }
    IEnumerator CSprintCoolState()
    {
        float time = 0f;
        bool maintain = true;
        while (true)
        {
            time += Time.deltaTime;
            if (time > sprintMaintainTimer && maintain)
            {
                maintain = false;
                isPlayerSprint = false;
                playerMoveSpeed = playerWalkSpeed;
            }

            if (time > sprintCoolTimer)
            {
                canSprint = true;
                yield break;
            }
            yield return null;
        }
    }

    public void LimitMovementSpeed()
    {
        if (isPlayerSprint) return;

        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        if (planeVelocity.magnitude > playerMoveLimitSpeed)
        {
            planeVelocity = planeVelocity.normalized;
            planeVelocity *= playerMoveLimitSpeed;
            planeVelocity.y = rigid.velocity.y;
            rigid.velocity = planeVelocity;
        }
    }



    /******************************************/
    /************** 물리 적용 ***************/
    /******************************************/



    public void Movement()
    {
        moveDirection = new Vector3(xMove, 0f, zMove);

        moveDirection = camTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        moveDirection = moveDirection.normalized;
        moveDirection *= playerMoveSpeed * planeCoefficient;

        //moveDirection = moveDirection.normalized;
    }

    public void SetGravity()
    {
        if (useGravity == false) return;
        if (isOnGround)
        {
            curGravity = minGravity;
        }
        else
        {
            curGravity += gravityIncreasemenet * Time.fixedDeltaTime;
            if (curGravity < maxGravity) curGravity = maxGravity;
        }
    }

    public void SlopeMovement()
    {
        isOnMaxAngleSlope = false;
        if (isOnGround)
        {
            float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            // 평지일 땐, 따로 계산 필요없음.
            if (Mathf.Abs(slopeAngle) < 0.1f) return;
            Vector3 slopeMoveDirection = moveDirection;
            slopeMoveDirection = Vector3.ProjectOnPlane(slopeMoveDirection, groundHit.normal);
            if (slopeAngle > slopeMaxAngle)
            {
                isOnMaxAngleSlope = true;
                slopeMoveDirection += slopeMoveDirection * (slopeAngle / 90.0f);
                slopeMoveDirection.y = slopeAngle * -0.2f;
            }
            if (Physics.Raycast(transform.position + Vector3.up * stepHeight * 0.5f, moveDirection, out slopeHit, slopeDetectDistance))
            {
                if (Vector3.Angle(slopeHit.normal, Vector3.up) > slopeMaxAngle)
                {
                    isOnMaxAngleSlope = true;
                    slopeMoveDirection = Vector3.zero;
                    slopeMoveDirection.y = slopeAngle * -0.2f;
                }
            }
            moveDirection = slopeMoveDirection;
        }
    }

    public void AirBlock()
    {
        if (!isOnGround)
        {
            if (Physics.CapsuleCast(transform.position + Vector3.up * playerBodyHeight, transform.position + Vector3.up * stepHeight,
                playerBodyRadius, moveDirection, playerBodyRadius + wallCheckDistanceDelta, groundLayer))
            {
                moveDirection = Vector3.zero;
            }
        }
    }

    public void SetRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Vector3 lookPosition = new Vector3(xMove, 0, zMove);
            lookPosition = camTransform.TransformDirection(lookPosition);
            lookPosition.y = 0f;
            if (lookPosition == Vector3.zero) return;
            moveRotation = Quaternion.LookRotation(lookPosition);
        }
    }

    public void ApplyMovementForce()
    {
        if (isOnGround)
        {
            moveDirection *= rigid.mass;
            rigid.AddForce(moveDirection, ForceMode.Force);
            return;
        }

        Vector3 airMovement = new Vector3(moveDirection.x, 0, moveDirection.z);
        airMovement *= airMovementMultiplier;
        airMovement.y = curGravity;
        airMovement *= rigid.mass;
        rigid.AddForce(airMovement, ForceMode.Force);
    }


    public void ApplyMovementRotation()
    {
        if (Quaternion.Angle(transform.rotation, moveRotation) > 1f)
        {
            if (isOnGround)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, Time.fixedDeltaTime * playerRotateSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, Time.fixedDeltaTime * playerRotateSpeed * airMovementMultiplier);
            }
        }
    }
}
