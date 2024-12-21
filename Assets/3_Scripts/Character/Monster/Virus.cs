using MonsterEnums;
using System.Collections.Generic;
using System.Collections;
using EffectEnums;
using UnityEngine;
using UnityEngine.AI;

public partial class Virus : CombatMonster
{
    [SerializeField] float nearCombatRange;
    [SerializeField] float farCombatRange;
    [Header("컴포넌트"),SerializeField] NavMeshAgent nav;
    [SerializeField] BaseDetecter detecter;
    [Header("유휴상태 유지시간"),SerializeField] float maintainIdleTime = 5f; 
    [SerializeField] bool isDoIdle = false;
    bool isDeathState = false; 
    bool isDoHitEffect = false; // 그로기.. 등 조작 불능 상태인 경우 넉백같은 효과가 작동하지 않도록 방지하는 변수
    bool isDoAnimation = false; // 피격, 공격.. 등 특정 행동 애니메이션 도중에는 작동하지 않도록 방지하는 변수

    [SerializeField] VirusSpread spread;
    [SerializeField] VirusRush rush;
    Sequence virusRoot = null;

    #region Life Cycle

    #region Awake
    protected override void Awake()
    {
        base.Awake();
        if (nav == null) nav = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        if (statusUI == null) statusUI = GetComponentInChildren<StandardMonsterStatusUI>();
    }
    #endregion

    #region Start
    protected override void Start()
    {
        base.Start();
        detecter.Setup(detectRange, (int)UtilEnums.LAYERS.PLAYER);
        nav.speed = monsterStat.Speed;
        spread.SetData(monsterStat);
        rush.SetData(monsterStat);  
    }

    protected override void CreateBTStates()
    {
        #region First BT States : Detect & Do Idle
        // Level 2 
        List<Node> doIdleStatesGroup = new List<Node>();
        ActionNode doMotionless = new ActionNode(DoMotionless);
        ActionNode doOnGuard = new ActionNode(DoOnGuard);
        doIdleStatesGroup.Add(doMotionless);
        doIdleStatesGroup.Add(doOnGuard);

        // Level 1
        List<Node> doDetectNearPlayer = new List<Node>();
        ActionNode detectNearPlayer = new ActionNode(DoDetectPlayer);
        ActionNode checkDoIdleState = new ActionNode(DoIdleState);
        RandomSelector randomDoIdle = new RandomSelector(doIdleStatesGroup);
        doDetectNearPlayer.Add(detectNearPlayer);
        doDetectNearPlayer.Add(checkDoIdleState);
        doDetectNearPlayer.Add(randomDoIdle);
        #endregion

        #region Second BT States : Check Far Combat Range
        // Level1
        ActionNode checkFarRange = new ActionNode(DoCheckFarAttackRange);
        ActionNode doMoveToPlayer = new ActionNode(DoMoveToTarget);
        List<Node> checkFarAttackRangeGroup = new List<Node>();
        checkFarAttackRangeGroup.Add(checkFarRange);
        checkFarAttackRangeGroup.Add(doMoveToPlayer);
        #endregion

        # region Third BT States : Check Far Attack Cool Time & Check Near Attack Range
        List<Node> checkFarAttackCoolTimeGroup = new List<Node>();
        
        ActionNode checkFarAttackCoolTime = new ActionNode(DoCheckCanFarAttack);
        ActionNode checkNearAttackRange = new ActionNode(DoCheckNearAttackRange);
        
        checkFarAttackCoolTimeGroup.Add(checkFarAttackCoolTime);
        checkFarAttackCoolTimeGroup.Add(checkFarAttackCoolTime);
        //checkFarAttackCoolTimeGroup.Add(doMoveToPlayer);
        #endregion

        // To Do ~~~~~~ Round Moving
        #region Forth BT States : Check Near Attack CoolTime 
        List<Node> checkNearCoolTimeGroup = new List<Node>();
        ActionNode doCheckNearAttackCoolTime = new ActionNode(DoCheckCanNearAttack);
        // ActionNode doMoveRoundPlayer = new ActionNode(DoCheckCanNearAttack);
        checkNearCoolTimeGroup.Add(doCheckNearAttackCoolTime);
        checkNearCoolTimeGroup.Add(doMoveToPlayer);
        #endregion

        ActionNode checkDoAnimation = new ActionNode(DoAnimation); // 애니메이션 작동중이면 작동 안함
        Selector doIdleSelector = new Selector(doDetectNearPlayer); // 첫 행동 갈래의 집합
        Selector doCheckFarAttackRangeSelector = new Selector(checkFarAttackRangeGroup); // 두번째 행동 갈래의 집합
        Sequence doCheckFarAttackCoolTimeSequence = new Sequence(checkFarAttackCoolTimeGroup); // 세번째 행동 갈래의 집합
        Sequence doNearAttackCoolTimeSequence = new Sequence(checkNearCoolTimeGroup); // 네번째 행동 갈래의 집합 : 마지막
        
        List<Node> virusBTGroup = new List<Node>();
        virusBTGroup.Add(checkDoAnimation);
        virusBTGroup.Add(doIdleSelector);
        virusBTGroup.Add(doCheckFarAttackRangeSelector);
        virusBTGroup.Add(doCheckFarAttackCoolTimeSequence);
        virusBTGroup.Add(doNearAttackCoolTimeSequence);


        // Root
        virusRoot = new Sequence(virusBTGroup);
    }
    #endregion

    #region FixedUpdate
    protected override void FixedUpdate()
    {
        if (isDeathState) return;
        virusRoot.Evaluate();
        statusUI.FixedExecute();
    }
    #endregion

    #endregion

    // Comon Method 
    NODESTATES DoMoveToTarget()
    {
        anim.SetInteger("MState", (int)STATES.MOVE);
        nav.SetDestination(detecter.GetTransform.position); 
        return NODESTATES.FAIL; 
    }

    #region Behaviours : Method

    #region First BT : Idle
    NODESTATES DoAnimation()
    {
        if (isDoAnimation) return NODESTATES.FAIL;
        return NODESTATES.SUCCESS;
    }

    NODESTATES DoDetectPlayer()
    {
        if (detecter.IsDetect())
            return NODESTATES.SUCCESS;
        return NODESTATES.FAIL;
    }

    NODESTATES DoIdleState() { return NODESTATES.FAIL; }

    NODESTATES DoOnGuard()
    {
        if(isDoIdle)
            return NODESTATES.FAIL;

        StartCoroutine(CDoIdle());
        anim.SetInteger("Idle", 1);
        anim.SetInteger("MState", (int)STATES.IDLE);
        detecter.ChangeRange(detectRange * 0.75f);
        return NODESTATES.FAIL;
    }

    NODESTATES DoMotionless()
    {
        if (isDoIdle)
            return NODESTATES.FAIL;

        StartCoroutine(CDoIdle());
        anim.SetInteger("Idle", 0);
        anim.SetInteger("MState", (int)STATES.IDLE);
        detecter.ChangeRange(detectRange);
        return NODESTATES.FAIL;
    }
    
    IEnumerator CDoIdle()
    {
        isDoIdle = true;
        yield return new WaitForSeconds(maintainIdleTime);
        isDoIdle = false;
    }

    #endregion

    #region Second BT : Check Distance & Move
    NODESTATES DoCheckFarAttackRange() { return (farCombatRange < detecter.GetDistance()) ? NODESTATES.FAIL : NODESTATES.SUCCESS; }
    #endregion

    #region Third BT : Far Attack
    NODESTATES DoCheckCanFarAttack()
    {
        return NODESTATES.SUCCESS;

        //if (spread.GetCoolDown)
        //{
        //    isDoAnimation = true;
        //    anim.SetInteger("Attack", 1);
        //    anim.SetInteger("MState",(int) STATES.ATTACK);
        //    return NODESTATES.FAIL;
        //}
    }
    NODESTATES DoCheckNearAttackRange()
    {
        if(nearCombatRange < detecter.GetDistance())
        {
            DoMoveToTarget();
            return NODESTATES.FAIL; 
        }
        return NODESTATES.SUCCESS;
    }
    public void DoSpread() { spread.Spread(); }
    public void StopSpread() { anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }
    #endregion

    #region Forth BT : Near Attack & Round Player
    NODESTATES DoCheckCanNearAttack()
    {
        if (rush.GetCoolDown)
        {
            transform.LookAt(detecter.GetTransform);
            isDoAnimation = true;
            anim.SetInteger("Attack",0);
            anim.SetInteger("MState", (int)STATES.ATTACK);
            return NODESTATES.FAIL;
        }
        return NODESTATES.SUCCESS;
    }

    public void DoRush() { rush.StartRush(); }
    public void StopRush() { rush.StopRush(); anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }
    #endregion

    #endregion

    #region State :  Relate Animation 
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData)
    {
        if (isDeathState) return;
        switch (_attackData.GetHitEffect)
        {
            case HIT_EFFECTS.STUN:
                StartCoroutine(CHitEffect(_attackData.EffectMaintainTime));
                anim.SetInteger("MState", (int)STATES.GROGGY);
                break;
            default:
                if (isDoHitEffect)
                    return;
                anim.SetInteger("MState", (int)STATES.HIT);
                break;
        }
    }

    IEnumerator CHitEffect(float _effectTime)
    {
        isDoAnimation = true;
        isDoHitEffect = true;
        yield return new WaitForSeconds(_effectTime);
        isDoHitEffect = false;
    }

    public override void Death()
    {
        // Base에서 Death 애니메이션과 Layer설정 변경함
        base.Death();
        isDeathState = true;
        statusUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 공격, 피격 .. 등 애니메이션 끝날 때 호출
    /// </summary>
    public void EscapeDoAnimation()  { isDoAnimation = false; }
    #endregion
}
