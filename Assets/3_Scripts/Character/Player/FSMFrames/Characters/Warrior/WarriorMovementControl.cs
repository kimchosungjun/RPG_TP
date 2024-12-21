using System.Collections;
using UnityEngine;
using PlayerEnums;
// ai 처리는 fixedupdate와 update가 좋다
// 이유는 동기화 처리때문에

public class WarriorMovementControl : PlayerMovementControl
{
    bool canPlayerCtrl = true;
    public bool CanPlayerCtrl { get { return canPlayerCtrl; } }

    #region Unity Life Cycle

    #region Init

    public override void Init(PlayerStat _playerStat)
    {
        base.Init(_playerStat);
        LinkMyComponent();
    }
    public void LinkMyComponent()
    {
        if (rigid == null) rigid = GetComponentInChildren<Rigidbody>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (collide == null) collide = GetComponentInChildren<CapsuleCollider>();
    }
    #endregion

    #region Setup
    public override void Setup()
    {
        LinkComponent();
        InitValues();
        CreateStates();
    }

    public void LinkComponent()
    {
        if (camTransform == null) camTransform = Camera.main.transform;
        if (collide != null) playerBodyRadius = collide.radius;
    }

    public void InitValues()
    {
        moveDirection = Vector3.zero;
        moveRotation = transform.rotation;
        groundDetectDistance = bodyTransform.position.y - playerBodyRadius + detectGroundDelta;
        slopeDetectDistance = stepHeight * 0.5f * 5f + playerBodyRadius;
        playerBodyHeight = collide.height;
    }

    protected override void CreateStates()
    {
        stateMachine = new PlayerStateMachine();
        attackCombo = new PlayerAttackCombo();
        playerStates = new PlayerState[(int)STATES.MAX];
        playerStates[(int)STATES.MOVEMENT] = new PlayerGroundMoveState(this);
        playerStates[(int)STATES.DASH] = new PlayerDashState(this);
        playerStates[(int)STATES.JUMP] = new PlayerJumpState(this);
        playerStates[(int)STATES.FALL] = new PlayerFallState(this);
        playerStates[(int)STATES.ATTACK] = new PlayerAttackState(this, attackCombo);
        playerStates[(int)STATES.SKILL] = new PlayerSkillState(this, attackCombo);
        playerStates[(int)STATES.ULTIMATESKILL] = new PlayerUltimateSkillState(this, attackCombo);
        playerStates[(int)STATES.HIT] = new PlayerHitState(this);
        //playerStates[(int)E_PLAYER_FSM.DEATH] = new (this, rigid, anim);
        playerStates[(int)STATES.INTERACTION] = new PlayerInteractionState(this);
        currentPlayerState = STATES.MOVEMENT;
        stateMachine.InitStateMachine(playerStates[(int)STATES.MOVEMENT]);
    }
    #endregion

    #region Execute
    public override void Execute()
    {
        if (canPlayerCtrl)
            stateMachine.Execute();
    }

    public override void FixedExecute()
    {
        stateMachine.FixedExecute();
    }
    #endregion

    #endregion

    #region DialogueTest Method
    public void TestDialogue(Transform _targetTransform)
    {
        if (!isOnGround) return;

        Vector3 direcition = _targetTransform.position - transform.position;
        direcition.y = 0f;
        float angle = Vector3.Angle(transform.forward, direcition);

        base.ChangeState(PlayerEnums.STATES.INTERACTION);
        
        if (angle < 10f)
        {
            transform.rotation = Quaternion.LookRotation(direcition);
            anim.SetBool("IsTurn",false);
        }
        else
        {
            StartCoroutine(CTurn(direcition));
            anim.SetBool("IsTurn",true);
        }
        anim.SetTrigger("Talk");
    }

    IEnumerator CTurn(Vector3 _direction)
    {
        float time = 0f;
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_direction);
        while (true)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, time / 1f);
            if (time > 1f)
                break;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
    #endregion

    public void MoveLock(bool _isMoveLock) 
    {
        canPlayerCtrl = _isMoveLock;
    }
}


