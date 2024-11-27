using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ai 처리는 fixedupdate와 update가 좋다
// 이유는 동기화 처리때문에

public partial class CharacterCtrl : MonoBehaviour
{
    PlayerState[] playerStates;
    PlayerStateMachine stateMachine;
    PlayerAttackCombo attackCombo;
    [SerializeField] E_PLAYER_FSM currentPlayerState = E_PLAYER_FSM.MAX;
    #region Unity Life Cycle
    void Start()
    {
        LinkComponent();
        InitValues();
        CreateStates();
    }

    public void LinkComponent()
    {
        if (rigid == null) rigid = GetComponentInChildren<Rigidbody>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (collide == null) collide = GetComponentInChildren<CapsuleCollider>();
        if (camTransform == null) camTransform = Camera.main.transform;
        if (collide != null) playerBodyRadius = collide.radius;
    }

    public void InitValues()
    {
        moveDirection = Vector3.zero;
        moveRotation = transform.rotation;
        playerMoveSpeed = playerWalkSpeed;
        groundDetectDistance = bodyTransform.position.y - playerBodyRadius + detectGroundDelta;
        slopeDetectDistance = stepHeight * 0.5f * 5f + playerBodyRadius;
        playerBodyHeight = collide.height;
    }

    public void CreateStates()
    {
        stateMachine = new PlayerStateMachine();
        attackCombo = new PlayerAttackCombo();

        playerStates = new PlayerState[(int)E_PLAYER_FSM.MAX];
        playerStates[(int)E_PLAYER_FSM.MOVEMENT] = new PlayerGroundMove(this);
        playerStates[(int)E_PLAYER_FSM.DASH] = new PlayerDash(this);
        playerStates[(int)E_PLAYER_FSM.JUMP] = new PlayerJump(this);
        playerStates[(int)E_PLAYER_FSM.FALL] = new PlayerFall(this);
        playerStates[(int)E_PLAYER_FSM.ATTACK] = new PlayerAttack(this, attackCombo);
        //playerStates[(int)E_PLAYER_FSM.SKILL] = new PlayerMovement(this, rigid, anim);
        //playerStates[(int)E_PLAYER_FSM.ULTIMATESKILL] = new PlayerMovement(this, rigid, anim);
        //playerStates[(int)E_PLAYER_FSM.HIT] = new PlayerMovement(this, rigid, anim);
        //playerStates[(int)E_PLAYER_FSM.DEATH] = new PlayerMovement(this, rigid, anim);

        currentPlayerState = E_PLAYER_FSM.MOVEMENT;
        stateMachine.InitStateMachine(playerStates[(int)E_PLAYER_FSM.MOVEMENT]);
    }

    public void ChangeState(E_PLAYER_FSM _E_PLAYER_NEW_FSM) { stateMachine.ChangeState(playerStates[(int)_E_PLAYER_NEW_FSM]); currentPlayerState = _E_PLAYER_NEW_FSM; }


    void Update()
    {
        stateMachine.Execute();

        //GroundCheck();
        //InputMovementKey();
        //LimitMovementSpeed();
        //UpdateAnimation();
        //// 공격
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    anim.SetInteger("AttackCombo", tempCombo);
        //    anim.SetTrigger("Attack");
        //    tempCombo += 1;
        //    if (tempCombo >= 3) tempCombo = 0;
        //}

        //anim.SetFloat("VerticalVelocity", rigid.velocity.y);

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    anim.SetTrigger("Hit");
        //}
    }

    void FixedUpdate()
    {
        stateMachine.FixedExecute();

        //Movement();
        //SlopeMovement();
        //AirBlock();
        //SetGravity();
        //SetRotation();
        //ApplyMovementForce();
        //ApplyMovementRotation();
    }
    #endregion
}
