using System.Collections;
using UnityEngine;
using MonsterEnums;
using EffectEnums;

public class StandardMonster : BaseMonster
{
    // Component & Float Values
    [Header("Status UI : Must Link"),SerializeField] protected StandardMonsterStatusUI statusUI = null;
    [Header("Range"),SerializeField] protected float nearCombatRange;
    [SerializeField] protected float farCombatRange;
    protected float maintainIdleTime = 5f;

    // Bool Values
    protected bool isBattle = false;
    protected bool isDoIdle = false;
    protected bool isDoHitEffect = false;
    protected bool isDoAnimation = false;
    protected bool isDoMoveNearPlayer = false;

    #region Override Life Cycle
    protected override void Awake()
    {
        base.Awake();
        if(statusUI==null)
            statusUI = GetComponentInChildren<StandardMonsterStatusUI>();   
        statusUI.Init();
        if (anim == null) anim = GetComponent<Animator>();
        if (monsterFinder == null) monsterFinder = GetComponentInChildren<MonsterFinder>();
    }

    protected override void Start()
    {
        base.Start();
        statusUI.Setup(this.transform, monsterStat);
        nav.speed = monsterStat.Speed;
        monsterFinder?.ChangeDetectLayer(UtilEnums.LAYERS.PLAYER);
    }

    protected override void CreateStates() { }
    #endregion

    #region Battle Field
    public override bool CanTakeDamageState()
    {
        if (IsInMonsterArea == false) return false;
        return true;
    }

    public override void AnnounceInMonsterArea()
    {
        base.AnnounceInMonsterArea();
        statusUI.DecideActiveState(true);
    }

    public override void AnnounceOutMonsterArea()
    {
        base.AnnounceOutMonsterArea();
        statusUI.DecideActiveState(false);
        ReturnToSpawnPosition();
    }
    #endregion

    // Relate State
    public override void AnnounceStatusUI() { statusUI.UpdateStatusData(); }

    #region Apply Damage (Stat & State)
    public override void ApplyStatTakeDamage(TransferAttackData _attackData)
    {
        base.ApplyStatTakeDamage(_attackData);
        if (isBattle == false)
            isBattle = true;
        EscapeReturnToSpawnPosition();
    }

    public override void ApplyMovementTakeDamage(TransferAttackData _attackData)
    {
        if (isDeathState) return;
    
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

    protected IEnumerator CHitEffect(float _effectTime)
    {
        isDoAnimation = true;
        isDoHitEffect = true;
        yield return new WaitForSeconds(_effectTime);
        ChangeAnimation(STATES.IDLE);
        isDoHitEffect = false;
        isDoAnimation = false;
    }

    public virtual void EscapeHitState()
    {
        if (isDeathState) return;
        isDoAnimation = false;
        isDoHitEffect = false;
        ChangeAnimation(STATES.IDLE);
    }
    #endregion

    #region Relate Alive State
    public override void Death()
    {
        base.Death(); // Base : Death Anim & Set Layer
        isDeathState = true;
        statusUI.DecideActiveState(false);
    }

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
    #endregion

    #region Relate Animations
    
    public virtual void ChangeAnimation(STATES _animState)
    {
        int animState = anim.GetInteger("MState");
        int changeAnimState = (int)_animState;
        if (animState == changeAnimState)
            return;
        anim.SetInteger("MState", changeAnimState);
    }

    public virtual void EscapeDoAnimation() { isDoAnimation = false; }
    #endregion
}
