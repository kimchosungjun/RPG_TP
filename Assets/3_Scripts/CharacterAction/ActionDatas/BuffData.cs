using UnityEngine;
using System;

public class BuffData
{
    [SerializeField, Tooltip("버프 효과")] protected E_BUFF_APPLY_STATS applyStatType;
    [SerializeField, Tooltip("어떤 스탯이 버프에 효과를 주는지")] protected E_BUFF_EFFECT_STATS effectStatType;
    [SerializeField, Tooltip("버프 지속성")] protected E_BUFF_COUNTINUITIES continuityType;

    public E_BUFF_APPLY_STATS GetApplyStatType { get { return applyStatType; } }
    public E_BUFF_EFFECT_STATS GetEffectStatType { get { return effectStatType; } }
    public E_BUFF_COUNTINUITIES GetContinuityType { get { return continuityType; } }

}

[Serializable]
public class PlayerBuffData : BuffData
{
    [SerializeField, Tooltip("버프 계수")] protected float[] buffMultipliers;
    public float GetBuffMultipliers(int _level) { return buffMultipliers[_level - 1]; }
}

[Serializable]
public class MonsterBuffData : BuffData
{
    [SerializeField, Tooltip("버프 계수")] protected float buffMultiplier;
    public float GetBuffMultiplier { get { return buffMultiplier; } }
}
