using MonsterEnums;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RangedMonster : StandardMonster
{
    [Header("Ranged Attack")]
    [SerializeField] VirusSpread spread;
    [SerializeField] MonsterTriggerAttackAction rush;
    Sequence rangedBTRoot = null;
    List<ITriggerAttack> triggerAttacks = new List<ITriggerAttack>();

    #region Start
    protected override void Start()
    {
        base.Start();
        spread.SetData(monsterStat);
        rush.SetData(monsterStat);
        triggerAttacks.Add(rush);
    }

    protected override void CreateStates()
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
        RandomSelector randomDoIdle = new RandomSelector(doIdleStatesGroup);
        doDetectNearPlayer.Add(detectNearPlayer);
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

        #region Root BT  States
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
        rangedBTRoot = new Sequence(virusBTGroup);
        #endregion
    }
    #endregion

    #region FixedUpdate
    protected override void FixedUpdate()
    {
        if (isDeathState) return;
        rangedBTRoot.Evaluate();
        statusUI.FixedExecute();
    }
    #endregion

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
        if (IsInMonsterArea == false || (Vector3.Distance(transform.position, FieldCenterPosition) > MonsterArea.GetRadius - 0.5f) )
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
        if (spread.GetCoolDown)
        {
            isDoAnimation = true;
            transform.rotation = Quaternion.LookRotation(monsterFinder.GetDirection());
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
    public void DoSpread() { spread.DoAttack(); sfxPlayer.PlayOneSFX(UtilEnums.SFXCLIPS.VIRUS_SPREAD_SFX); }
    public void StopSpread() { anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }

    #endregion

    #region Forth BT : Near Attack & Round Player
    NODESTATES DoCheckCanNearAttack()
    {
        if (rush.GetCoolDown)
        {
            transform.rotation = Quaternion.LookRotation(monsterFinder.GetDirection());
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

    public void DoRush() { rush.DoAttack(); }
    public void StopRush() { rush.StopAttack(); anim.SetInteger("MState", (int)STATES.IDLE); isDoAnimation = false; }
    #endregion

    #endregion

    // Comon Method 
    NODESTATES DoMoveToTarget()
    {
        bool continueMove = false;
        if (isDoMoveNearPlayer)
        {
            if (spread.GetCoolDown || rush.GetCoolDown)
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

    public override void EscapeHitState()
    {
        base.EscapeHitState();
        triggerAttacks[0].InActiveTrigger();
    }

    public override void Death()
    {
        base.Death();
        nav.ResetPath();
        triggerAttacks[0].InActiveTrigger();
    }
}
