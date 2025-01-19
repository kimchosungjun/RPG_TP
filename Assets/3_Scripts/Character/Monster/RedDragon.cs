using System.Collections;
using System.Collections.Generic;
using DragondStateClasses;
using UnityEngine;
using MonsterEnums;
using UnityEngine.AI;

public class RedDragon : EliteMonster
{
    [SerializeField] RedDragonAttackControl attackControl;
    [SerializeField] NavMeshAgent nav;
    [SerializeField] float attackRange = 5f;
    bool isPassiveRotate = false;
    DRAGON_PHASE currentPhase = DRAGON_PHASE.FIRST;

    bool isScream = false;

    #region ENUM
    public enum DRAGON_STATE
    {
        IDLE=0,
        MOVE,
        FIRST,
        SECOND,
        THIRD,
        SPECIAL,
        HIT,
        GROGGY,
        DEATH
    }

    public enum DRAGON_ANIM
    {
        IDLE=0,
        MOVE=1,
        SCREAM=2,
        GROGGY=3,
        HIT=4,
        ATTACK=5, // 0 : Basic, 1 : Claw, 2 : Flame, 3 : Chase Flame
        FLY=6,
        LAND=7,
        DEATH=8
    }

    public enum DRAGON_PHASE
    {
        FIRST,
        SECOND,
        THIRD
    }
    #endregion

    #region Set Animation & State

    [SerializeField] int currentStateIndex = 0;
    DragonState [] currentStates;
    MonsterStateMachine stateMachine;
    
    protected override void CreateBTStates()
    {
        stateMachine = new MonsterStateMachine();
        currentStates = new DragonState[9];
        currentStates[(int)DRAGON_STATE.IDLE] = new DragonIdleState(this);
        currentStates[(int)DRAGON_STATE.MOVE] = new DragonMoveState(this);
        currentStates[(int)DRAGON_STATE.FIRST] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.SECOND] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.THIRD] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.SPECIAL] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.HIT] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.GROGGY] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.DEATH] = new DragonState(this);

        stateMachine.InitStateMachine(currentStates[0]);
    }

    public void ChangeState(DRAGON_STATE _newState)
    {
        currentStateIndex = (int)_newState;
        stateMachine.ChangeState(currentStates[(int)_newState]);
    }

    public void SetAnimation(DRAGON_ANIM _state)
    {
        anim.SetInteger("State", (int)_state);
    }

    public void SetAttackAnimation(DRAGON_ANIM _state, int _attackIndex)
    {
        anim.SetInteger("State", (int)_state);
        anim.SetInteger("AttackState", _attackIndex);
    }

    public void ClearState()
    {
        anim.SetInteger("State", 0);
        anim.SetInteger("AttackState", 0);
    }
    #endregion

    #region Idle Behaviour
    
    public void CheckPlayerInArea()
    {
        if (isInMonsterArea && isScream == false)
        {
            isScream = true;
            SetAnimation(DRAGON_ANIM.SCREAM);
        }
    }

    public void AfterScream()
    {
        if (isInMonsterArea)
        {
            SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI.TurnOn();
            ChangeState(DRAGON_STATE.MOVE);
        }
        else
            isScream = false;
    }

    #endregion

    #region Check Enemy In Range
    public bool IsInAttackRange()
    {
        float detectionAngle = 60f; 
        int enemyLayer = 1 << (int)UtilEnums.LAYERS.PLAYER;
        Vector3 enemy = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        if(Vector3.Distance(enemy, transform.position) <= attackRange)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
            if (hits.Length == 0) return false;
            Vector3 directionToTarget = (hits[0].transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle <= detectionAngle)
                return true;
            return false;
        }
        return false;
    }

    #endregion

    #region Check Phase
    public NODESTATES IsSamePhase(DRAGON_PHASE _phase)
    {
        if (currentPhase == _phase) return NODESTATES.SUCCESS;
        return NODESTATES.FAIL;
    }
    public NODESTATES CheckFirstPhase() { return IsSamePhase(DRAGON_PHASE.FIRST); }
    public NODESTATES CheckSecondPhase() { return IsSamePhase(DRAGON_PHASE.SECOND); }
    public NODESTATES CheckThirdPhase() { return IsSamePhase(DRAGON_PHASE.THIRD); }
    #endregion

    #region Movement

    // Player Postion
    Vector3 destPosition;

    // Orbit Movement
    float currentAngle = 0;
    float orbitDistance = 20f;
    float orbitSpeed = 5f;
    float rotateSpeed = 10f;

    public void ChasePlayer()
    {
        destPosition = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        if (Vector3.Distance(transform.position, destPosition) <= attackRange)
        {
            if (isPassiveRotate == false)
            {
                nav.speed = 0.2f;
                isPassiveRotate = true;
                nav.updateRotation = false;
            }
            OrbitMove(destPosition);
            OrbitLookAt(destPosition);
        }
        else
        {
            if (isPassiveRotate)
            {
                nav.speed = 3f;
                nav.updateRotation = true;
                isPassiveRotate = false;
            }
            nav.SetDestination(destPosition);
        }
    }

    public void OrbitMove(Vector3 _center)
    {
        currentAngle+= Time.fixedDeltaTime;
        float xOffset = Mathf.Cos(currentAngle) * orbitDistance;
        float zOffset = Mathf.Sin(currentAngle) * orbitDistance;
        _center += new Vector3(xOffset, 0, zOffset);
        nav.SetDestination(_center);    
    }

    public void OrbitLookAt(Vector3 _center)
    {
        Vector3 directionToPlayer = (_center - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotateSpeed);
    }

    #endregion
  
    #region Check Condition
    public bool CheckCoolDown()
    {
        return attackControl.GetCoolDown;
    }

    #endregion

    #region First

    TransferAttackData attackData = new TransferAttackData();
   
    #endregion

    #region Second
    public NODESTATES CanDoFirstAttack()
    {
        if (CheckCoolDown()) return NODESTATES.SUCCESS;
        return NODESTATES.FAIL;
    }

    public NODESTATES DoBasicAttack()
    {
        //attackControl.BasicAttack();    
        return NODESTATES.SUCCESS;
    }

    #endregion
    
    public override bool CanTakeDamageState() { return true; }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) { }


    #region Life Cycle
    protected override void Awake()
    {
        base.Awake();
        if (nav == null) nav = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start(); 
        statusUI = SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI;
        //attackControl.SetData(monsterStat);
    }

    protected override void FixedUpdate()
    {
        currentStates[currentStateIndex].FixedExecute();
    }
    #endregion
}
