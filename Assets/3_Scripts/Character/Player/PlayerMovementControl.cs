using System.Collections;
using UnityEngine;
using PlayerEnums;
public abstract class PlayerMovementControl : MonoBehaviour
{
    /******************************************/
    /************* 캐릭터 이동 **************/
    /**************   변수들    ***************/
    /******************************************/

    #region Relate Hit
    public EffectEnums.HIT_EFFECTS HitCombo { get; set; } = EffectEnums.HIT_EFFECTS.NONE;
    public float HitEffectTime { get; set; } = 0f;
    #endregion

    #region Component
    [Header("필수 연결 컴포넌트")]
    [SerializeField] protected Transform bodyTransform;
    [SerializeField] protected CapsuleCollider collide;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected Animator anim;
    protected Transform camTransform;
    public Rigidbody GetRigid { get { return rigid; } }
    public Animator GetAnim { get { return anim; } }
    public Transform GetBodyTransform { get { return bodyTransform; } }
    #endregion

    #region Movement

    /******************************************/
    /*********** 공통 이동 변수 *************/
    /******************************************/
    protected float playerMoveLimitSpeed = 5f;

    /******************************************/
    /************** 평면 이동 ****************/
    /******************************************/
    protected float playerRotateSpeed = 12f;
    protected float moveCoefficient = 10f; // 이동 계수
    protected float xMove;
    protected float zMove;
    protected float playerMoveSpeed;
    protected Vector3 moveDirection;
    protected Quaternion moveRotation;
    public Quaternion SetMoveRotation { set { moveRotation = value; } } 

    public float XMove { get { return xMove; } set { xMove = value; } }
    public float ZMove { get { return zMove; } set { zMove = value; } }
    public float PlayerMoveSpeed { get { return playerMoveSpeed; } }
    public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }
    public Quaternion MoveRotation { get { return moveRotation; } set { moveRotation = value; } }

    /******************************************/
    /***************** 점프  ******************/
    /******************************************/
    protected float playerJumpForce;
    protected float airMovementMultiplier = 0.2f;
    public float PlayerJumpForce { get { return playerJumpForce; } }
    public float AirMovementMultiplier { get { return airMovementMultiplier; } }

    /******************************************/
    /***************** 대쉬  ******************/
    /******************************************/
    protected float dashCoolTimer = 0.3f;
    protected float playerDashForce;
    protected bool isPlayerDashing = false;
    protected bool canDash = true;
    #endregion

    #region Common Detect
    /******************************************/
    /*********** 공통 검출 변수 *************/
    /******************************************/
    protected int groundLayer = 1 << 3 | 1 << 6; // 3은 벽, 6은 땅
    protected int monsterLayer = 1 << 7;
    protected float playerBodyRadius;
    protected float playerBodyHeight;
    public float PlayerBodyHegiht { get { return PlayerBodyHegiht; } }

    /******************************************/
    /************** 검출 여부 ****************/
    /******************************************/
    protected bool isOnGround = false;
    protected bool isOnMaxAngleSlope = false;

    public bool IsOnGround { get { return isOnGround; } set { isOnGround = value; } }
    public bool IsOnMaxAngleSlope { get { return isOnMaxAngleSlope; } }

    /******************************************/
    /**************** 땅 검출 ****************/
    /******************************************/
    [Header("저항값")]
    [SerializeField, Tooltip("땅 저항 값")] protected float groundDrag = 6;
    [SerializeField, Tooltip("공기 저항 값")] protected float airDrag = 1;
    protected float detectGroundDelta = 0.15f;

    public float GroundDrag { get { return groundDrag; } }
    public float AirDrag { get { return airDrag; } }


    protected float groundDetectDistance;
    protected RaycastHit groundHit;

    /******************************************/
    /************ 경사로 검출 ***************/
    /******************************************/
    [Header("경사로")]
    [SerializeField] protected float slopeMaxAngle = 50f;
    [SerializeField] protected float stepHeight = 0.2f;

    protected float slopeDetectDistance;
    protected RaycastHit slopeHit;

    /******************************************/
    /************** 벽 검출 ******************/
    /******************************************/
    protected float wallCheckDistanceDelta = 0.15f;
    #endregion

    #region Gravity
    [Header("중력")]
    [SerializeField] protected bool useGravity = true;
    protected float gravityIncreasemenet = -9.8f;
    protected float curGravity = 0f;
    protected float minGravity = -5f;
    protected float maxGravity = -100f;
    #endregion

    #region State
    [SerializeField] protected float attackRange;

    protected PlayerState[] playerStates;
    protected PlayerStateMachine stateMachine;
    protected PlayerAttackCombo attackCombo;
    [SerializeField] protected STATES currentPlayerState = STATES.MAX;
    protected bool canTakeDamageState = true;
    public bool CanTakeDamage { get { return canTakeDamageState; } set { canTakeDamageState = value; } }

    protected bool isOnAirState = false;
    public bool IsOnAirState { get { return isOnAirState; }  set { isOnAirState = value; } }
    #endregion

    /******************************************/
    /************** 속도 조절  ***************/
    /**************   땅 검출   ***************/
    /******************************************/

    #region Common Method : Limit Speed, Check Ground
    public void GroundCheck()
    {
        isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down,
            out groundHit, groundDetectDistance, groundLayer);
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

    #region Rotation, Gravity, MoveDirection
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
    #endregion

    #region Prevent Under Monster
    public void MonsterCheck()
    {
        RaycastHit monsterHit;
        if (Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down,
         out monsterHit, groundDetectDistance, monsterLayer))
        {
            Vector3 reverseForward = transform.forward * -1f + Vector3.up * 0.5f;
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
            ChangeState(PlayerEnums.STATES.MOVEMENT);
        else
            ChangeState(PlayerEnums.STATES.FALL);

        StartCoroutine(CDashCooling());
    }

    IEnumerator CDashCooling()
    {
        float time = 0f;
        // To Do ~~~~~ 
        // 스테미나 줄어들게 만드는 기능 추가
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
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * curGravity * rigid.mass, ForceMode.Force);
    }
    #endregion

    #region Jump
    public void AddforceForJump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    }
    #endregion

    /*===========================*/
    /*========= 상태 변경 ==========*/
    /*========= 값 초기 설정 ========*/
    /*========== 피격 설정 =========*/
    /*===========================*/

    #region Virtual
    public virtual void ChangeState(STATES _E_PLAYER_NEW_FSM)
    {
        stateMachine.ChangeState(playerStates[(int)_E_PLAYER_NEW_FSM]);
        currentPlayerState = _E_PLAYER_NEW_FSM;
    }

    /// <summary>
    ///  Awake
    /// </summary>
    public virtual void Init(PlayerStat _playerStat)
    {
        playerMoveSpeed = _playerStat.Speed;
        playerMoveLimitSpeed = playerMoveSpeed;
        playerJumpForce = _playerStat.JumpSpeed;
        playerDashForce = _playerStat.DashSpeed;
    }


    public virtual void ApplyTakeDamageState(EffectEnums.HIT_EFFECTS _hitEffect, float _maintainTime)
    {
        if (isOnAirState)
        {
            switch (_hitEffect)
            {
                case EffectEnums.HIT_EFFECTS.NONE:
                    break;
                default:
                    HitCombo = EffectEnums.HIT_EFFECTS.FALLDOWN;
                    anim.SetInteger("HitCombo", (int)EffectEnums.HIT_EFFECTS.FALLDOWN);
                    ChangeState(STATES.HIT);
                    break;
            }
        }
        else
        {
            if (_hitEffect == EffectEnums.HIT_EFFECTS.NONE)
                return;

            anim.SetInteger("HitCombo", (int)_hitEffect);
            HitCombo = _hitEffect;
            HitEffectTime = _maintainTime;
            ChangeState(STATES.HIT);
        }
    }
    #endregion

    /*===========================*/
    /*=========추상 메서드=========*/
    /*===========================*/

    #region Abstract
    /// <summary>
    /// Start
    /// </summary>
    public abstract void Setup();
    /// <summary>
    /// Update
    /// </summary>
    public abstract void Execute();
    /// <summary>
    /// FixedUpdate
    /// </summary>
    public abstract void FixedExecute();
    /// <summary>
    /// 상태 정의
    /// </summary>
    protected abstract void CreateStates();
    #endregion

    #region Change State By Animation
    public void AttackCooling() { ChangeState(STATES.MOVEMENT); }
    public void SkillCooling() { ChangeState(STATES.MOVEMENT); }
    public void UltimateSkillCooling() { ChangeState(STATES.MOVEMENT); }
    public virtual void Death() { ChangeState(STATES.DEATH); }
    #endregion

}
