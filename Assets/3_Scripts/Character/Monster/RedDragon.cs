using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterEnums;
using UnityEngine.AI;

public class RedDragon : EliteMonster
{
    [SerializeField] RedDragonAttackControl attackControl;
    [SerializeField] NavMeshAgent nav;
    [SerializeField] float attackRange = 5f;
    bool isPassiveRotate = false;

    int currentState = 0;

    #region ENUM
    enum DRAGON_ANIM
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

    DRAGON_PHASE currentPhase = DRAGON_PHASE.FIRST;
    bool isConstraint = false; // Hit, Groggy
    Selector rootSelector;

    Sequence firstRoot;
    protected override void CreateBTStates()
    {
        // Common
        ActionNode checkInAttackRange = new ActionNode(IsInAttackRange);
        ActionNode chasePlayer = new ActionNode(ChasePlayer);

        // First : Check Constraint Control
        ActionNode constraintNode =new ActionNode(BeingConstraint);

        // Second : FirstPhase
        ActionNode checkFirstPhase = new ActionNode(CheckFirstPhase); 

        List<Node> firstPhaseNodes = new List<Node>();
        firstPhaseNodes.Add(checkFirstPhase);

        ActionNode checkCanDoFirstAttack = new ActionNode(CanDoFirstAttack);

        List<Node> firstPhaseCoolDownNodes = new List<Node>();
        firstPhaseCoolDownNodes.Add(checkCanDoFirstAttack);

        List<Node> firstPhaseAttackConditionNodes = new List<Node>();
        firstPhaseAttackConditionNodes.Add(checkInAttackRange);
        firstPhaseAttackConditionNodes.Add(chasePlayer);
        Selector firstPhaseAttackCondition = new Selector(firstPhaseAttackConditionNodes);
        firstPhaseCoolDownNodes.Add(firstPhaseAttackCondition);
        
        ActionNode doBasicAttack = new ActionNode(DoBasicAttack);   
        firstPhaseCoolDownNodes.Add(doBasicAttack);
        Sequence firstCoolDown = new Sequence(firstPhaseCoolDownNodes);
        
        firstPhaseNodes.Add(firstCoolDown);

        List<Node> firstPhaseCoolTimeNodes = new List<Node>();
        firstPhaseCoolTimeNodes.Add(chasePlayer);    
        Sequence fistCoolTime = new Sequence(firstPhaseCoolTimeNodes);
        
        firstPhaseNodes.Add(fistCoolTime);

        Sequence firstPhase = new Sequence(firstPhaseNodes);
        firstRoot = firstPhase;

        //List<Node> behaviourNodes = new List<Node>();
        //behaviourNodes.Add(constraintNode);

        //Sequence redDragon = new Sequence(behaviourNodes);
        //rootSelector = new Selector();
    }

    #region Check Enemy In Range
    public NODESTATES IsInAttackRange()
    {
        float detectionAngle = 60f; 
        int enemyLayer = 1 << (int)UtilEnums.LAYERS.PLAYER;
        Vector3 enemy = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        if(Vector3.Distance(enemy, transform.position) <= attackRange)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
            if (hits.Length == 0) return NODESTATES.FAIL;
            Vector3 directionToTarget = (hits[0].transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle <= detectionAngle)
                return NODESTATES.SUCCESS;
            return NODESTATES.FAIL;
        }
        return NODESTATES.FAIL;
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
    float orbitDistance = 3f;
    float orbitSpeed = 5f;
    float rotateSpeed = 10f;
    public NODESTATES ChasePlayer()
    {
        if(currentState != 1)
        {
            currentState = 1;
            anim.SetInteger("State", currentState);
        }

        destPosition = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        if (Vector3.Distance(transform.position, destPosition) <= attackRange)
        {
            if (isPassiveRotate)
            {
                isPassiveRotate = false;
                nav.angularSpeed = 1080f;
            }
            OrbitMove(destPosition);
            OrbitLookAt(destPosition);
        }
        else
        {
            if(isPassiveRotate == false)
            {
                isPassiveRotate = true;
                nav.angularSpeed = 0f;
            }
            nav.SetDestination(destPosition);
        }
        return NODESTATES.SUCCESS; 
    }

    public void OrbitMove(Vector3 _center)
    {
        float angle = Time.time * orbitSpeed; // 현재 시간에 따른 각도 계산
        float xOffset = Mathf.Cos(angle) * orbitDistance;
        float zOffset = Mathf.Sin(angle) * orbitDistance;
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
    public NODESTATES BeingConstraint()
    {
        if (isConstraint)
            return NODESTATES.SUCCESS;
        else
            return NODESTATES.FAIL;
    }
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
        statusUI = SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI;
        base.Start();
        attackControl.SetData(monsterStat);
    }

    protected override void FixedUpdate()
    {
        if (firstRoot != null) firstRoot.Evaluate();
    }
    #endregion
}
