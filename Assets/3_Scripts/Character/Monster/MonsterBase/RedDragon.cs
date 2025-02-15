using System.Collections;
using DragondStateClasses;
using UnityEngine;
using UnityEngine.AI;

public class RedDragon : EliteMonster
{
    #region Variable

    [Header("RedDragon Component")]
    [SerializeField, Range(0,10f)] float attackRange = 4f;
    [SerializeField] CapsuleCollider coll;
    [SerializeField] RedDragonAttackControl attackControl;
    public NavMeshAgent GetNav { get { return nav; } }
    public RedDragonAttackControl GetAttackControl { get { return attackControl; } }

    // Relate State & Anim
    bool isScream = false;
    bool isFlying = false;
    bool isInvincible = false; // current glide state =>  true else false

    public bool IsInvincible { get { return isInvincible; } set { isInvincible = value; } }   

    // Glide Point
    int glidePoint = 0;
    int maxGlidePointIndex = 0;
    int glideCycleCnt = 0;
    int glideMaxCycleCnt = 3;
    [Header("Glide Point : Later Erase")]
    [SerializeField] Vector3[] glidePostions;

    public SFXPlayer GetSFXPlayer { get { return sfxPlayer; } } 
    #endregion

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
        DEATH,
        CALM
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

    #region Set State
    DragonState currentState = null;
    DragonState [] allStates = null;
    MonsterStateMachine stateMachine;
    [SerializeField] DRAGON_STATE curState;
    protected override void CreateStates()
    {
        stateMachine = new MonsterStateMachine();
        allStates = new DragonState[9];
        allStates[(int)DRAGON_STATE.IDLE] = new DragonIdleState(this);
        allStates[(int)DRAGON_STATE.MOVE] = new DragonMoveState(this);
        allStates[(int)DRAGON_STATE.BATTLE] = new DragonAttackState(this);
        allStates[(int)DRAGON_STATE.TAKEOFF] = new DragonTakeOffState(this);
        allStates[(int)DRAGON_STATE.GLIDE] = new DragonGlideState(this);
        allStates[(int)DRAGON_STATE.LAND] = new DragonLandState(this);
        allStates[(int)DRAGON_STATE.HIT] = new DragonHitState(this);
        allStates[(int)DRAGON_STATE.DEATH] = new DragonDeathState(this);
        allStates[(int)DRAGON_STATE.CALM] = new DragonCalmState(this); 
        currentState = allStates[(int)DRAGON_STATE.IDLE];
        stateMachine.InitStateMachine(allStates[0]);
    }

    public void ChangeState(DRAGON_STATE _newState)
    {
        curState = _newState;
        currentState = allStates[(int)_newState];
        stateMachine.ChangeState(allStates[(int)_newState]);
    }

    public override void Death()
    {
        base.Death();
        ChangeState(DRAGON_STATE.DEATH);
    }

    public override void Revival()
    {
        isDeathState = false;
        isRecovery = false;
        isGoOffAggro = false;
        isScream = false;
        isFlying = false;
        isInvincible = false;

        base.Revival();
        if(allStates != null)
            ChangeState(DRAGON_STATE.IDLE);
    }

    #endregion

    #region Set Animation

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
        Vector3 glideDestination = SpawnPosition;
        if (glidePoint >= maxGlidePointIndex)
        {
            attackControl.DoOrbitFlameAttack();
            glideCycleCnt += 1;
            if (glideCycleCnt >= glideMaxCycleCnt)
            {
            
                ChangeState(DRAGON_STATE.LAND);
                yield break;
            }
            glidePoint = 0;
        }
        glideDestination += glidePostions[glidePoint];
        nav.SetDestination(glideDestination);
        while (true)
        {
            if(Vector3.Distance(transform.position, glideDestination) < 1f)
            {
                glidePoint += 1;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(CGliding());
    }
    public void EndLand() { ChangeState(DRAGON_STATE.MOVE); }

    public void SetCollideState(bool _activeState) { coll.enabled = _activeState;  }

    // Relate Hit
    public void ExitHitState() { ChangeState(DRAGON_STATE.MOVE); }

    #endregion

    #region Check Range
    
    public void CheckPlayerInArea()
    {
        if (IsInMonsterArea && isScream == false)
        {
            isScream = true;
            isInvincible = true;
            sfxPlayer.PlayOneSFX(UtilEnums.SFXCLIPS.DRAGON_GROWL_SFX);
            SetAnimation(DRAGON_ANIM.SCREAM);
        }
    }

    public bool IsInAttackRange()
    {
        float detectionAngle = 30f;
        int enemyLayer = 1 << (int)UtilEnums.LAYERS.PLAYER;
        Vector3 enemy = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        if (Vector3.Distance(enemy, transform.position) <= attackRange)
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

    #region Behaviour

    // Meet Player : Once
    public void AfterScream()
    {
        isInvincible = false;
        if (IsInMonsterArea)
        {
            SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI.TurnOn();
            ChangeState(DRAGON_STATE.MOVE);
        }
        else
        {
            isScream = false;
            SetAnimation(DRAGON_ANIM.IDLE);
        }
    }

    // Chase Player
    Vector3 destPosition = Vector3.zero;
    public void ChasePlayer()
    {
        destPosition = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        nav.SetDestination(destPosition);
    }

    #endregion

    #region Take Damage

    // Relate Hit Condition
    bool isChangeEffectTime = false;
    float effectMaintainTime = 0f;
    
    public float EffectMaintainTime 
    {
        get
        { 
            return effectMaintainTime; 
        }
        set
        {
            effectMaintainTime = value;
            isChangeEffectTime = !isChangeEffectTime; 
        }
    }
    public bool IsChangeEffectTime 
    {
        get 
        {
            return isChangeEffectTime;
        }
        set
        {
            isChangeEffectTime = value;
        }
    }

    EffectEnums.HIT_EFFECTS currentEffect = EffectEnums.HIT_EFFECTS.NONE;
    public EffectEnums.HIT_EFFECTS GetCurrentHitEffect { get { return currentEffect; } }

    public override bool CanTakeDamageState() { return isInvincible ? false : true; }

    public override void ApplyStatTakeDamage(TransferAttackData _attackData)
    {
        base.ApplyStatTakeDamage(_attackData);
        CheckFlyState();
    }

    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) 
    {
        currentEffect = _attackData.GetHitEffect;
        if (currentEffect != EffectEnums.HIT_EFFECTS.NONE)
        {
            EffectMaintainTime = _attackData.EffectMaintainTime;
            ChangeState(DRAGON_STATE.HIT);
        }
    }

    public void CheckFlyState()
    {
        if (isFlying == false && monsterStat.GetCurrentHPPercent() <= 30)
        {
            isInvincible = true;
            isFlying = true;
            ChangeState(DRAGON_STATE.TAKEOFF);
        }
    }

    public override void AnnounceGroggyState(float _groggyTime)
    {
        if (isDeathState) return;
        if (curState == DRAGON_STATE.GLIDE || curState == DRAGON_STATE.TAKEOFF || curState == DRAGON_STATE.LAND)
            return;
        EffectMaintainTime = _groggyTime;
        currentEffect = EffectEnums.HIT_EFFECTS.STUN; 
        if(curState != DRAGON_STATE.HIT)
            ChangeState(DRAGON_STATE.HIT);
    }

    #endregion

    #region Calm State
    public bool GetIsAllPlayerDeathState { get { return allPlayerDeath; } }
    public override void AnnounceOutMonsterArea()
    {
        base.AnnounceOutMonsterArea();
        IsInMonsterArea = false;
        if (isDeathState==false)
            ChangeState(DRAGON_STATE.CALM);
    }
    public override void EscapeReturnToSpawnPosition() { ChangeState(DRAGON_STATE.CALM); }
    public override void AnnounceAllPlayerDeath()
    {
        base.AnnounceAllPlayerDeath();
        if(isDeathState==false)
            ChangeState(DRAGON_STATE.CALM);
    }

    protected override IEnumerator CGoOffAggro()
    {
        isGoOffAggro = true;
        nav.SetDestination(SpawnPosition);
        nav.stoppingDistance = 0;
        SetAnimation(DRAGON_ANIM.MOVE);
        while (true)
        {
            if (isGoOffAggro == false) yield break;
            if (nav.remainingDistance < toOriginalStopDistance) break;
            yield return new WaitForFixedUpdate();
        }
        nav.stoppingDistance = toPlayerStopDistance;
        SetAnimation(DRAGON_ANIM.IDLE);
        isGoOffAggro = false;
    }

    public void ResetState()
    {
        SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI.TurnOff();
        IsInMonsterArea = false;
        isScream = false;
        isFlying = false;
        isInvincible = false;
    }

    #endregion

    #region Life Cycle
    protected override void Awake()
    {
        base.Awake();
        if (nav == null) nav = GetComponent<NavMeshAgent>();
        if(coll==null) coll = GetComponent<CapsuleCollider>();
        maxGlidePointIndex = glidePostions.Length;
        eliteGauge = new EliteGauge(this);
    }

    protected override void Start()
    {
        base.Start(); 
        statusUI = SharedMgr.UIMgr.GameUICtrl.GetBossStatusUI;
        attackControl.SetData(monsterStat);
    }

    protected override void FixedUpdate()
    {
        currentState.FixedExecute();
    }
    #endregion
}