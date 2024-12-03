using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public partial class CharacterCtrl : MonoBehaviour
{
    [SerializeField] int tempCombo;
    #region Component
    [Header("필수 연결 컴포넌트")]
    [SerializeField] Transform camTransform;
    [SerializeField] Transform bodyTransform;
    [SerializeField] CapsuleCollider collide;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    public Transform CamTransform { get { return camTransform; } }
    public Transform BodyTransform { get { return bodyTransform; } }
    public CapsuleCollider Collider { get { return collide; } }
    public Rigidbody Rigid { get { return rigid; } }
    public Animator Anim { get { return anim; } }
    #endregion

    #region Movement

    /******************************************/
    /*********** 공통 이동 변수 *************/
    /******************************************/
    [Header("이동"),SerializeField] float playerMoveLimitSpeed = 5f;

    /******************************************/
    /************** 평면 이동 ****************/
    /******************************************/
    [SerializeField] float playerWalkSpeed = 5f;
    [SerializeField] float playerRotateSpeed = 12f;
    [SerializeField] float moveCoefficient = 10f;
    float xMove;
    float zMove;
    float playerMoveSpeed;
    Vector3 moveDirection;
    Quaternion moveRotation;

    public float XMove { get { return xMove; } set { xMove = value; } }
    public float ZMove { get { return zMove; } set { zMove = value; } }
    public float PlayerMoveSpeed { get { return playerMoveSpeed; } }
    public Vector3 MoveDirection { get { return moveDirection; }  set { moveDirection = value; } }
    public Quaternion MoveRotation { get { return moveRotation; } set { moveRotation = value; } }

    /******************************************/
    /***************** 점프  ******************/
    /******************************************/
    [SerializeField] float playerJumpForce;
    [SerializeField, Range(0, 1f)] float airMovementMultiplier;
    public float PlayerJumpForce { get { return playerJumpForce; } }
    public float AirMovementMultiplier { get { return airMovementMultiplier; } }    

    /******************************************/
    /***************** 대쉬  ******************/
    /******************************************/
    [SerializeField] float playerDashForce;
    [SerializeField] float dashCoolTimer = 0.3f;
    bool isPlayerDashing = false;
    bool canDash = true;
    #endregion

    /******************************************/
    /*********** 공통 검출 변수 *************/
    /******************************************/
    int groundLayer = 1 << 3 | 1 << 6; // 3은 벽, 6은 땅
    int monsterLayer = 1 << 7;
    float playerBodyRadius;
    [SerializeField] float playerBodyHeight;
    public int GroundLayer { get { return groundLayer; } }
    public float PlayerBodyRadius { get { return playerBodyRadius; } }
    public float PlayerBodyHegiht { get { return PlayerBodyHegiht; } }

    /******************************************/
    /************** 검출 여부 ****************/
    /******************************************/
    bool isOnGround = false;
    bool isOnMaxAngleSlope = false;

    public bool IsOnGround { get { return isOnGround; } set { isOnGround = value; } }
    public bool IsOnMaxAngleSlope { get { return isOnMaxAngleSlope; } }

    /******************************************/
    /**************** 땅 검출 ****************/
    /******************************************/
    [Header("검출")]
    [SerializeField, Tooltip("땅 검출 시, 추가 검출 거리")] float detectGroundDelta = 0.1f;
    [SerializeField, Tooltip("땅 저항 값")] float groundDrag = 6;
    [SerializeField, Tooltip("공기 저항 값")] float airDrag = 1;

    public float GroundDrag { get { return groundDrag; } }
    public float AirDrag { get { return airDrag; } }

 
    float groundDetectDistance;
    RaycastHit groundHit;

    /******************************************/
    /************ 경사로 검출 ***************/
    /******************************************/
    [SerializeField] float slopeMaxAngle = 50f;
    [SerializeField] float stepHeight = 0.2f;
   
    float slopeDetectDistance;
    RaycastHit slopeHit;

    /******************************************/
    /************** 벽 검출 ******************/
    /******************************************/
    [SerializeField] float wallCheckDistanceDelta;

    [Header("중력")]
    [SerializeField] bool useGravity = true;
    [SerializeField] float gravityIncreasemenet = -9.8f;
    [SerializeField] float curGravity = 0f;
    [SerializeField] float minGravity = 0f;
    [SerializeField] float maxGravity =0f;


    /******************************************/
    /************** 속도 조절  ***************/
    /**************   땅 검출    ***************/
    /******************************************/
    #region Common Method : Limit Speed, Check Ground
    public void GroundCheck()
    {
        isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down,
            out groundHit, groundDetectDistance, groundLayer);
        anim.SetBool("IsOnGround", isOnGround);
    }
    public void LimitMovementSpeed()
    {
        if (isPlayerDashing) return;

        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        if (isOnGround)
        {
            planeVelocity.y = rigid.velocity.y;
            if (planeVelocity.magnitude > playerMoveLimitSpeed)
            {
                planeVelocity = planeVelocity.normalized;
                planeVelocity *= playerMoveLimitSpeed;
                rigid.velocity = planeVelocity;
            }
        }
        else
        {
            // 공중에선 중력이 작용하기에 중력값 제외하고 속도 조정
            if (planeVelocity.magnitude > playerMoveLimitSpeed)
            {
                planeVelocity = planeVelocity.normalized;
                planeVelocity *= playerMoveLimitSpeed;
                planeVelocity.y = rigid.velocity.y;
                rigid.velocity = planeVelocity;
            }
        }
    }

    
    #endregion

    /****************************************/
    /************** 물리 적용 **************/
    /****************************************/

    #region Common Method : Rotation, Gravity, AddForce, MoveDirection
    public void SetMoveDirection()
    {
        moveDirection = new Vector3(xMove, 0f, zMove);
        moveDirection = camTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;
        moveDirection *= playerMoveSpeed * moveCoefficient;
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
    #endregion

    #region Ground Movement 
    public void SetSlopeMoveDirection()
    {
        if (isOnGround)
        {
            float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            // 평지일 땐, 따로 계산 필요없음.
            if (Mathf.Abs(slopeAngle) < 0.1f) { isOnMaxAngleSlope = false; return; }
            Vector3 slopeMoveDirection = moveDirection;
            slopeMoveDirection = Vector3.ProjectOnPlane(slopeMoveDirection, groundHit.normal);
            if (slopeAngle > slopeMaxAngle)
            {
                isOnMaxAngleSlope = true;
                slopeMoveDirection += slopeMoveDirection * (slopeAngle / 90.0f);
                slopeMoveDirection.y = slopeAngle * -0.2f;
            }
            else
            {
                isOnMaxAngleSlope = false;
                moveDirection = slopeMoveDirection;
                return;
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
    public void ApplyGroundForce()
    {
        moveDirection *= rigid.mass;
        rigid.AddForce(moveDirection, ForceMode.Force);
    }
    public void ApplyGroundRotation()
    {
        if (Quaternion.Angle(transform.rotation, moveRotation) > 1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, Time.fixedDeltaTime * playerRotateSpeed);
    }
    #endregion

    #region Air Movement
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
    public void ApplyAirForce()
    {
        Vector3 airMovement = new Vector3(moveDirection.x, 0, moveDirection.z);
        airMovement *= airMovementMultiplier;
        airMovement.y = curGravity;
        airMovement *= rigid.mass;
        rigid.AddForce(airMovement, ForceMode.Force);
    }
    public void ApplyAirRotation()
    {
        if (Quaternion.Angle(transform.rotation, moveRotation) > 1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, Time.fixedDeltaTime * playerRotateSpeed * airMovementMultiplier);
    }

    public void MonsterCheck()
    {
        RaycastHit monsterHit;
        if (Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down,
         out monsterHit, groundDetectDistance, monsterLayer))
        {
            Vector3 reverseForward = transform.forward * -1f + Vector3.up *0.5f;
            rigid.AddForce(reverseForward * 0.1f, ForceMode.Impulse);
        }
    }
    #endregion

    #region Dash
    /// <summary>
    /// 대쉬로 
    /// </summary>
    public void Dash()
    {
        if (canDash)
        {
            canDash = false;
            isPlayerDashing = true;
            Vector3 dashDirection = transform.forward;
            dashDirection.y = 0;
            dashDirection = dashDirection.normalized;
            if (isOnGround)
                dashDirection = Vector3.ProjectOnPlane(dashDirection, groundHit.normal);
            rigid.AddForce(dashDirection * playerDashForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 대쉬 쿨타임은 코루틴으로 구현
    /// </summary>
    public void DashCooling()
    {
        isPlayerDashing = false;

        if (isOnGround)
            ChangeState(E_PLAYER_STATES.MOVEMENT);
        else
            ChangeState(E_PLAYER_STATES.FALL);

        StartCoroutine(CDashCooling());
    }

    IEnumerator CDashCooling()
    {
        float time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time > dashCoolTimer)
            {
                canDash = true;
                break;
            }
            yield return null;
        }
    }

    public void ApplyGravityForce()
    {
        if (isOnGround) return;
        //rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * curGravity * rigid.mass, ForceMode.Force);
    }
    #endregion

    #region Attack & Skill & UltimateSkill
    public void AttackCooling() { ChangeState(E_PLAYER_STATES.MOVEMENT); }

    public void SkillCooling() { ChangeState(E_PLAYER_STATES.MOVEMENT); }

    public void UltimateSkillCooling() { ChangeState(E_PLAYER_STATES.MOVEMENT); }
    #endregion
}
