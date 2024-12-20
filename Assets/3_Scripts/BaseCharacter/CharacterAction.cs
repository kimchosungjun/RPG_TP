using UnityEngine;

/// <summary>
/// 버프, 디버프, 근거리, 원거리 공격의 Base
/// </summary>
public abstract class CharacterAction : MonoBehaviour
{
    public abstract void DoAction();
    public abstract void StopAction();
}


