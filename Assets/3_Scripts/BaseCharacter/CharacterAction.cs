using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction : MonoBehaviour
{
    public abstract void DoAction();
    public abstract void StopAction();

    /// <summary>
    /// 쿨타임이 지났으면 True
    /// </summary>
    /// <returns></returns>
    public virtual bool IsCoolDown() { return false; }
    public virtual float GetActionCoolTime() { return 0; }
    public virtual void StartCoolDown() { }
}


