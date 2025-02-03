using MonsterEnums;
using System.Collections.Generic;
using System.Collections;
using EffectEnums;
using UnityEngine;


public class ChestMonster : StandardMonster
{
    [Header("Range")]
    [SerializeField] float nearCombatRange;
    [SerializeField] float farCombatRange;

    float maintainIdleTime = 5f;
    bool isDoIdle = false;
    bool isDoHitEffect = false; // 그로기.. 등 조작 불능 상태인 경우 넉백같은 효과가 작동하지 않도록 방지하는 변수
    bool isDoAnimation = false; // 피격, 공격.. 등 특정 행동 애니메이션 도중에는 작동하지 않도록 방지하는 변수
    bool isDoMoveNearPlayer = false;

    [Header("Chest Attack")]
    [SerializeField] ChestBite bite;
    [SerializeField] ChestRush rush;
    Sequence chestBTRoot = null;

    #region Life Cycle

    #region Awake
    protected override void Awake()
    {
        base.Awake();
        if (anim == null) anim = GetComponent<Animator>();
        if (monsterFinder == null) monsterFinder = GetComponentInChildren<MonsterFinder>();
    }
    #endregion

    #region Start
    protected override void Start()
    {
        base.Start();
        nav.speed = monsterStat.Speed;
        rush.SetData(monsterStat);
        bite.SetData(monsterStat);
        monsterFinder?.ChangeDetectLayer(UtilEnums.LAYERS.PLAYER);
    }

    protected override void CreateStates()
    {
        // Near : Bite
        // Far : Rush

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
        List<Node> checkFarAttackRangeGroup = new List<Node>();
        checkFarAttackRangeGroup.Add(checkFarRange);
        #endregion

        # region Third BT States : Check Far Attack Cool Time & Check Near Attack Range
        List<Node> checkFarAttackCoolTimeGroup = new List<Node>();
        ActionNode checkFarAttackCoolTime = new ActionNode(DoCheckCanFarAttack);
        ActionNode checkNearAttackRange = new ActionNode(DoCheckNearAttackRange);

        checkFarAttackCoolTimeGroup.Add(checkFarAttackCoolTime);
        checkFarAttackCoolTimeGroup.Add(checkNearAttackRange);
        #endregion

        #region Forth BT States : Check Near Attack CoolTime 
        List<Node> checkNearCoolTimeGroup = new List<Node>();
        ActionNode doCheckNearAttackCoolTime = new ActionNode(DoCheckCanNearAttack);
        ActionNode doMoveRoundPlayer = new ActionNode(DoMoveNearPlayer);
        checkNearCoolTimeGroup.Add(doCheckNearAttackCoolTime);
        checkNearCoolTimeGroup.Add(doMoveRoundPlayer);
        #endregion

        ActionNode checkDoAnimation = new ActionNode(DoAnimation); // 애니메이션 작동중이면 작동 안함
        ActionNode checkGoOutOfBounds = new ActionNode(DoCheckGoOutOfBounds);
        Selector doIdleSelector = new Selector(doDetectNearPlayer); // 첫 행동 갈래의 집합
        Selector doCheckFarAttackRangeSelector = new Selector(checkFarAttackRangeGroup); // 두번째 행동 갈래의 집합
        Sequence doCheckFarAttackCoolTimeSequence = new Sequence(checkFarAttackCoolTimeGroup); // 세번째 행동 갈래의 집합
        Sequence doNearAttackCoolTimeSequence = new Sequence(checkNearCoolTimeGroup); // 네번째 행동 갈래의 집합 : 마지막

        List<Node> virusBTGroup = new List<Node>();
        virusBTGroup.Add(checkDoAnimation);
        virusBTGroup.Add(checkGoOutOfBounds);
        virusBTGroup.Add(doIdleSelector);
        virusBTGroup.Add(doCheckFarAttackRangeSelector);
        virusBTGroup.Add(doCheckFarAttackCoolTimeSequence);
        virusBTGroup.Add(doNearAttackCoolTimeSequence);

        // Root
        chestBTRoot = new Sequence(virusBTGroup);
    }
    #endregion

    #region FixedUpdate
    protected override void FixedUpdate()
    {
        if (isDeathState) return;
        chestBTRoot.Evaluate();
        statusUI.FixedExecute();
    }
    #endregion

    #endregion

    // Comon Method 
    NODESTATES DoMoveToTarget()
    {
        bool continueMove = false;
        if (isDoMoveNearPlayer)
        {
            if (rush.GetCoolDown || bite.GetCoolDown)
                continueMove = true;

            if (continueMove == false)
            {
                DoMoveBack();
                return NODESTATES.FAIL;
            }

            nav.speed = monsterStat.Speed;
            nav.updateRotation = true;
            isDoMoveNearPlayer = false;
        }

        if (monsterFinder.GetDistance() < 2f)
        {
            ChangeAnimation(STATES.IDLE);
            Vector3 direction = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position - this.transform.position;
            direction.y = 0;
            direction = direction.normalized;
            Quaternion.LookRotation(direction);

            if (nav.updateRotation)
                nav.updateRotation = false;
            nav.SetDestination(this.transform.position);
        }
        else
        {
            if (nav.updateRotation == false)
                nav.updateRotation = true;
            ChangeAnimation(STATES.MOVE);
            nav.SetDestination(SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position);
        }
        return NODESTATES.FAIL;
    }

    #region Behaviours : Method

    #region First BT : Idle
    NODESTATES DoAnimation()
    {
        if (isDeathState) return NODESTATES.FAIL;

        if (isDoAnimation) return NODESTATES.FAIL;

        return NODESTATES.SUCCESS;
    }

    NODESTATES DoCheckGoOutOfBounds()
    {
        if (IsInMonsterArea == false || (Vector3.Distance(transform.position, FieldCenterPosition) > MonsterArea.GetRadius - 0.5f))
        {
            ReturnToSpawnPosition();
            if (isBattle)
                isBattle = false;
            return NODESTATES.FAIL;
        }
        EscapeReturnToSpawnPosition();
        return NODESTATES.SUCCESS;
    }

    NODESTATES DoDetectPlayer()
    {
        if (monsterFinder.IsDetect() || isBattle)
            return NODESTATES.SUCCESS;
        return NODESTATES.FAIL;
    }

    NODESTATES DoIdleState() { return NODESTATES.FAIL; }

    NODESTATES DoOnGuard()
    {
        if (isDoIdle || isDeathState)
            return NODESTATES.FAIL;

        StartCoroutine(CDoIdle());
        anim.SetInteger("Idle", 1);
        ChangeAnimation(STATES.IDLE);
        return NODESTATES.FAIL;
    }

    NODESTATES DoMotionless()
    {
        if (isDoIdle)
            return NODESTATES.FAIL;

        StartCoroutine(CDoIdle());
        anim.SetInteger("Idle", 0);
        ChangeAnimation(STATES.IDLE);
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

    NODESTATES DoCheckFarAttackRange()
    {
        if (monsterFinder.DetectInSihgt(farCombatRange) == false)
        {
            DoMoveToTarget();
            return NODESTATES.FAIL;
        }
        return NODESTATES.SUCCESS;
    }

    #endregion

    #region Third BT : Far Attack
    NODESTATES DoCheckCanFarAttack()
    {
        if (rush.GetCoolDown)
        {
            isDoAnimation = true;
            anim.SetInteger("Attack", 1);
            anim.SetInteger("MState", (int)STATES.ATTACK);
            return NODESTATES.FAIL;
        }
        return NODESTATES.SUCCESS;
    }
    NODESTATES DoCheckNearAttackRange()
    {
        if (monsterFinder.DetectInSihgt(nearCombatRange) == false)
        {
            DoMoveToTarget();
            return NODESTATES.FAIL;
        }
        return NODESTATES.SUCCESS;
    }
    public void DoRush() { rush.DoAttack(); }
    public void StopRush() { anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }

    #endregion

    #region Forth BT : Near Attack & Round Player
    NODESTATES DoCheckCanNearAttack()
    {
        if (bite.GetCoolDown)
        {
            transform.LookAt(SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position);
            isDoAnimation = true;
            anim.SetInteger("Attack", 0);
            anim.SetInteger("MState", (int)STATES.ATTACK);
            return NODESTATES.FAIL;
        }
        return NODESTATES.SUCCESS;
    }

    NODESTATES DoMoveNearPlayer()
    {
        if (isDoMoveNearPlayer == false)
        {
            nav.speed = 0.25f;
            nav.updateRotation = false;
            isDoMoveNearPlayer = true;
            nav.stoppingDistance = 0f;
        }

        DoMoveBack();
        return NODESTATES.SUCCESS;
    }

    public void DoMoveBack()
    {
        Vector3 direction = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
        direction = direction.normalized * 1.5f;

        if (Vector3.Distance((transform.position - direction), FieldCenterPosition) < MonsterArea.GetRadius - 0.6f)
            nav.SetDestination(transform.position - direction);
        else
            nav.ResetPath();
    }

    public void DoBite() { bite.StopAttack(); }
    public void StopBite() { bite.StopAttack(); anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }
    #endregion

    #endregion

    #region State :  Relate Animation 
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData)
    {
        if (isDeathState) return;
        bite.StopAttack();

        if (isDoHitEffect == false) nav.ResetPath();
        switch (_attackData.GetHitEffect)
        {
            case HIT_EFFECTS.STUN:
                StartCoroutine(CHitEffect(_attackData.EffectMaintainTime));
                anim.SetInteger("MState", (int)STATES.GROGGY);
                break;
            default:
                if (isDoHitEffect)
                    return;
                isDoHitEffect = true;
                anim.SetInteger("MState", (int)STATES.HIT);
                break;
        }
    }

    IEnumerator CHitEffect(float _effectTime)
    {
        isDoAnimation = true;
        isDoHitEffect = true;
        yield return new WaitForSeconds(_effectTime);
        ChangeAnimation(STATES.IDLE);
        isDoHitEffect = false;
        isDoAnimation = false;
    }

    public void EscapeHitState()
    {
        if (isDeathState) return;
        isDoAnimation = false;
        isDoHitEffect = false;
        ChangeAnimation(STATES.IDLE);
    }

    public override void Death()
    {
        // Base에서 Death 애니메이션과 Layer설정 변경함
        base.Death();
        isDeathState = true;
        statusUI.DecideActiveState(false);
    }

    /// <summary>
    /// 공격, 피격 .. 등 애니메이션 끝날 때 호출
    /// </summary>
    public void EscapeDoAnimation() { isDoAnimation = false; }

    public void ChangeAnimation(STATES _animState)
    {
        int animState = anim.GetInteger("MState");
        int changeAnimState = (int)_animState;
        if (animState == changeAnimState)
            return;
        anim.SetInteger("MState", changeAnimState);
    }

    #endregion

    public override void Revival()
    {
        base.Revival();
        nav.speed = monsterStat.Speed;
        isBattle = false;
        isDoIdle = false;
        isDoHitEffect = false;
        isDoAnimation = false;
        isDoMoveNearPlayer = false;
    }

    public override void ApplyStatTakeDamage(TransferAttackData _attackData)
    {
        base.ApplyStatTakeDamage(_attackData);
        EscapeReturnToSpawnPosition();
    }
}
