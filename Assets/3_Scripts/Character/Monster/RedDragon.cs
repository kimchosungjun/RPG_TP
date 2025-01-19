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
    [SerializeField] float attackRange = 4f;
    DRAGON_PHASE currentPhase = DRAGON_PHASE.FIRST;
    public NavMeshAgent GetNav { get { return nav; } }

    bool isScream = false;

    int glidePoint = 0;
    int maxGlidePointIndex = 0;
    int glideCycleCnt = 0;
    int glideMaxCycleCnt = 3;
    [SerializeField] Vector3[] glidePostions;

    #region ENUM
    public enum DRAGON_STATE
    {
        IDLE=0,
        MOVE,
        BATTLE,
        TAKEOFF,
        GLIDE,
        LAND,
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
        TAKEOFF=6,
        GLIDE=7,
        LAND=8,
        DEATH=9
    }

    public enum DRAGON_PHASE
    {
        FIRST,
        SECOND,
        THIRD
    }
    #endregion

    #region Set Animation & State

    [SerializeField] DRAGON_STATE currentStateIndex = 0;
    DragonState [] currentStates;
    MonsterStateMachine stateMachine;
    
    protected override void CreateBTStates()
    {
        stateMachine = new MonsterStateMachine();
        currentStates = new DragonState[9];
        currentStates[(int)DRAGON_STATE.IDLE] = new DragonIdleState(this);
        currentStates[(int)DRAGON_STATE.MOVE] = new DragonMoveState(this);
        currentStates[(int)DRAGON_STATE.BATTLE] = new DragonAttackState(this);
        currentStates[(int)DRAGON_STATE.TAKEOFF] = new DragonTakeOffState(this);
        currentStates[(int)DRAGON_STATE.GLIDE] = new DragonGlideState(this);
        currentStates[(int)DRAGON_STATE.LAND] = new DragonLandState(this);
        currentStates[(int)DRAGON_STATE.HIT] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.GROGGY] = new DragonState(this);
        currentStates[(int)DRAGON_STATE.DEATH] = new DragonState(this);

        stateMachine.InitStateMachine(currentStates[0]);
    }

    public void ChangeState(DRAGON_STATE _newState)
    {
        currentStateIndex = _newState;
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

    // Relate Attack Animation

    public void ChargeAttackEnergy(float _attackSpeed = 0.3f) { anim.SetFloat("AttackSpeed", _attackSpeed); }

    public void EndChargeAttackEnergy() { anim.SetFloat("AttackSpeed", 1); }

    public void ExitAttackState() { ChangeState(DRAGON_STATE.MOVE); }

    // Relate Fly
    public void EndTakeOff() { ChangeState(DRAGON_STATE.GLIDE); }
    public void GlideAttack() { StartCoroutine(CGliding()); }
    IEnumerator CGliding()
    {
        if (glidePoint >= maxGlidePointIndex)
        {
            DoGuidedFireAttack();
            glideCycleCnt += 1;
            if (glideCycleCnt >= glideMaxCycleCnt)
            {
            
                ChangeState(DRAGON_STATE.LAND);
                yield break;
            }
            glidePoint = 0;
        }
        nav.SetDestination(glidePostions[glidePoint]);
        while (true)
        {
            if(Vector3.Distance(transform.position, glidePostions[glidePoint]) < 1f)
            {
                glidePoint += 1;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(CGliding());
    }

    public void DoGuidedFireAttack()
    {
        Transform tf = SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.GUIDED_FIRE);
        tf.transform.position = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
    }

    public void EndLand() { ChangeState(DRAGON_STATE.MOVE); }
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
        float detectionAngle = 30f; 
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

    Vector3 destPosition = Vector3.zero;

    public void ChasePlayer()
    {
        destPosition = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        nav.SetDestination(destPosition);
    }



    #endregion
  
    #region Check Condition
    public bool CheckCoolDown()
    {
        return attackControl.GetCoolDown;
    }

    #endregion

    public void DoAttack()
    {
        
    }

    
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
        maxGlidePointIndex = glidePostions.Length;
    }

    protected override void Start()
    {
        base.Start(); 
        statusUI = SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI;
        //attackControl.SetData(monsterStat);
    }

    protected override void FixedUpdate()
    {
        currentStates[(int)currentStateIndex].FixedExecute();
    }
    #endregion
}

/*
#region Orbit

Vector3 destPosition;
float currentAngle = 0;
float orbitDistance = 20f;
float orbitSpeed = 5f;
float rotateSpeed = 10f;

public void OrbitMove(Vector3 _center)
{
    currentAngle += Time.fixedDeltaTime;
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
*/