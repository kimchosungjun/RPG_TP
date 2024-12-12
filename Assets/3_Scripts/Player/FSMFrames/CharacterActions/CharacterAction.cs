using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction : MonoBehaviour
{
    /// <summary>
    /// 공격력, 방어력, 스피드, 체력
    /// </summary>
    protected BaseStat targetStat = null;
    [SerializeField] protected E_PARTICLES actionParticleKey = E_PARTICLES.NONE;
    [SerializeField] protected float actionCoolTime;

    /// <summary>
    /// 반드시 재정의해서 사용할 것
    /// </summary>
    public abstract void DoAction();
    public virtual void SetTargetStat(BaseStat _targetStat) { this.targetStat = _targetStat; }
}

/// <summary>
/// 즉발 버프와 지속 버프
/// </summary>
public abstract class EffectAction : CharacterAction
{
    //[SerializeField] protected E_BUFF_STAT statusEffectType;
    public override void DoAction() {}
    public abstract void DoStatus();   
}

/// <summary>
/// 근거리와 원거리
/// </summary>
public abstract class AttackAction : CharacterAction
{
    public override void DoAction() { }
    public abstract void DoAttack();
}
