using System.Collections;
using UnityEngine;
using PlayerEnums;

public class ArcherMovementControl : PlayerMovementControl
{
    bool canPlayerCtrl = true;
    public bool CanPlayerCtrl { get { return canPlayerCtrl; } }

    #region Unity Life Cycle

    #region Init

    public override void Init(PlayerStat _playerStat)
    {
        base.Init(_playerStat);
        LinkParentComponent();
    }
    public void LinkParentComponent()
    {
        if (rigid == null) rigid = GetComponentInParent<Rigidbody>();
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (collide == null) collide = GetComponentInParent<CapsuleCollider>();
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
        playerStates[(int)STATES.JUMP] = new ArcherJumpState(this);
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
}
