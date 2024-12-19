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
    [Header("유휴상태 유지시간"),SerializeField] float maintainIdleTime; 
    bool isDoIdle = false;
    bool isDeathState = false; 
    bool isDoHitEffect = false; // 그로기.. 등 조작 불능 상태인 경우 넉백같은 효과가 작동하지 않도록 방지하는 변수
    bool isDoAnimation = false; // 피격, 공격.. 등 특정 행동 애니메이션 도중에는 작동하지 않도록 방지하는 변수

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
        SetAttackData();
    }

    protected override void CreateBTStates()
    {
        // Level1 
        //ActionNode checkCurrentStateAction = new ActionNode(DoCheckHitState);
        //ActionNode patrolAction = new ActionNode(DoPatrol);

        #region First BT States : Detect & Do Idle
        // Level 2 
        List<Node> doIdleStates = new List<Node>();
        ActionNode doMotionless = new ActionNode(DoMotionless);
        ActionNode doOnGuard = new ActionNode(DoOnGuard);
        doIdleStates.Add(doMotionless);
        doIdleStates.Add(doOnGuard);

        // Level 1
        List<Node> doDetectNearPlayer = new List<Node>();
        ActionNode detectNearPlayer = new ActionNode(DoDetectPlayer);
        ActionNode checkDoIdleState = new ActionNode(DoIdleState);
        RandomSelector randomDoIdle = new RandomSelector(doIdleStates);
        doDetectNearPlayer.Add(detectNearPlayer);
        doDetectNearPlayer.Add(checkDoIdleState);
        doDetectNearPlayer.Add(randomDoIdle);
        #endregion

        List<Node> virusBTGroup = new List<Node>();
        Selector doIdleSelector = new Selector(doDetectNearPlayer); // 첫 행동 갈래의 집합

        // Root
        virusRoot = new Sequence(virusBTGroup);
    }
    #endregion

    #region FixedUpdate
    protected override void FixedUpdate()
    {
        if (isDeathState) return;
        //virusRoot.Evaluate();
        statusUI.FixedExecute();
    }
    #endregion

    #endregion

    #region Behaviours : Method

    #region First BT : Idle
    NODESTATES DoDetectPlayer()
    {
        if(detecter.IsDetect())
            return NODESTATES.SUCCESS;
        return NODESTATES.FAIL;
    }

    NODESTATES DoIdleState() { if (isDoIdle) return NODESTATES.SUCCESS; else return NODESTATES.FAIL; }

    NODESTATES DoOnGuard()
    {
        StartCoroutine(CDoIdle());
        anim.SetInteger("MState", (int)STATES.IDLE);
        detecter.ChangeRange(detectRange * 0.75f);
        return NODESTATES.SUCCESS;
    }

    NODESTATES DoMotionless()
    {
        StartCoroutine(CDoIdle());
        anim.SetInteger("MState", (int)STATES.IDLE);
        detecter.ChangeRange(detectRange);
        return NODESTATES.SUCCESS;
    }
    
    IEnumerator CDoIdle()
    {
        isDoIdle = true;
        yield return new WaitForSeconds(maintainIdleTime);
        isDoIdle = false;
    }

    #endregion

    #region Second BT : Move & Attack
    NODESTATES DoCheckAttackRange() { return (farCombatRange < detecter.GetDistance()) ? NODESTATES.FAIL : NODESTATES.SUCCESS; }
    NODESTATES DoMoveToTarget() { nav.SetDestination(detecter.GetTransform.position);  return NODESTATES.SUCCESS; }
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
