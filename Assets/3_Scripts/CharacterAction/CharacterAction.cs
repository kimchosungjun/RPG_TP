using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction 
{
    protected Stat targetStat = null;
    /// <summary>
    /// 반드시 재정의해서 사용할 것
    /// </summary>
    public abstract void DoAction();
    public virtual void SetTargetStat(Stat _targetStat) { this.targetStat = _targetStat; }
}

/// <summary>
/// 버프와 디버프
/// </summary>
public abstract class StatControlAction : CharacterAction
{
    public override void DoAction() {}
    public abstract void DoStatControl();   
}

/// <summary>
/// 근거리와 원거리
/// </summary>
public abstract class AttackAction : CharacterAction
{
    public override void DoAction() { }
    public abstract void DoAttack();
}
