using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction : MonoBehaviour
{
    public abstract void DoAction();
    public abstract void StopAction();
}

/// <summary>
/// 즉발 버프와 지속 버프
/// </summary>
public abstract class BuffAction : CharacterAction
{
    protected CharacterStatCtrl statCtrl;
    public CharacterStatCtrl StatCtrl { get { return statCtrl; } }
    public override void DoAction() { DoBuff(); }
    public override void StopAction() { StopBuff(); }
    public abstract void DoBuff();
    public abstract void StopBuff();
}

/// <summary>
/// 근거리와 원거리
/// </summary>
public abstract class AttackAction : CharacterAction
{
    // DoAction이 작동하면 각자 SO에서 계수와 효과를 불러온다.
    // 현재 스탯에 접근하여 스탯을 불러온다.
    // 계수와 스탯을 조합하여 데이터를 전달한다.
    // DoAction에서 공격을 활성화 or 생성한다.
    // MovementCtrl에서 해당 Action을 관리하는게 좋을듯하다.
    // 애니메이션에서 설정을 관리하는건 아닌거같음. 그냥 활성화만 하는게 나을듯.

    public override void DoAction() { DoAttack(); }
    public override void StopAction() { StopAttack(); }
    public abstract void DoAttack();
    public abstract void StopAttack();
}
