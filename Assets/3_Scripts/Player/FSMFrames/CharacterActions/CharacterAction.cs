using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction : MonoBehaviour
{
    public abstract void DoAction();
}

/// <summary>
/// 즉발 버프와 지속 버프
/// </summary>
public abstract class BuffAction : CharacterAction
{
    public override void DoAction() { DoBuff(); }
    public abstract void DoBuff();   
}

/// <summary>
/// 근거리와 원거리
/// </summary>
public abstract class AttackAction : CharacterAction
{
    public override void DoAction() { DoAttack(); }
    public abstract void DoAttack();
    public abstract void StopAttack();
}
