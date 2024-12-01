using System.Collections;
using UnityEngine;

// ai 처리는 fixedupdate와 update가 좋다
// 이유는 동기화 처리때문에

public partial class CharacterCtrl : MonoBehaviour
{
    PlayerState[] playerStates;
    PlayerStateMachine stateMachine;
    PlayerAttackCombo attackCombo;
    [SerializeField] E_PLAYER_FSM currentPlayerState = E_PLAYER_FSM.MAX;
    bool canPlayerCtrl = true;
    public bool CanPlayerCtrl { get { return canPlayerCtrl; } }

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
        playerStates[(int)E_PLAYER_FSM.MOVEMENT] = new PlayerGroundMoveState(this);
        playerStates[(int)E_PLAYER_FSM.DASH] = new PlayerDashState(this);
        playerStates[(int)E_PLAYER_FSM.JUMP] = new PlayerJumpState(this);
        playerStates[(int)E_PLAYER_FSM.FALL] = new PlayerFallState(this);
        playerStates[(int)E_PLAYER_FSM.ATTACK] = new PlayerAttackState(this, attackCombo);
        playerStates[(int)E_PLAYER_FSM.SKILL] = new PlayerSkillState(this,attackCombo);
        playerStates[(int)E_PLAYER_FSM.ULTIMATESKILL] = new PlayerUltimateSkillState(this, attackCombo);
        playerStates[(int)E_PLAYER_FSM.HIT] = new PlayerHitState(this);
        //playerStates[(int)E_PLAYER_FSM.DEATH] = new (this, rigid, anim);
        playerStates[(int)E_PLAYER_FSM.INTERACTION] = new PlayerInteractionState(this);

        currentPlayerState = E_PLAYER_FSM.MOVEMENT;
        stateMachine.InitStateMachine(playerStates[(int)E_PLAYER_FSM.MOVEMENT]);
    }

    public void ChangeState(E_PLAYER_FSM _E_PLAYER_NEW_FSM) { stateMachine.ChangeState(playerStates[(int)_E_PLAYER_NEW_FSM]); currentPlayerState = _E_PLAYER_NEW_FSM; }


    void Update()
    {
        if(canPlayerCtrl)
            stateMachine.Execute();

        //GroundCheck();
        //InputMovementKey();
        //LimitMovementSpeed();
        //UpdateAnimation();
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

    public void TestDialogue(Transform _targetTransform)
    {
        if (!isOnGround) return;

        Vector3 direcition = _targetTransform.position - transform.position;
        direcition.y = 0f;
        float angle = Vector3.Angle(transform.forward, direcition);

        ChangeState(E_PLAYER_FSM.INTERACTION);
        
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

    public void MoveLock(bool _isMoveLock) 
    {
        canPlayerCtrl = _isMoveLock;
    }
}
