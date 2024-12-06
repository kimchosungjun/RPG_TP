using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "PlayerBuffAction", menuName = "Player/Action/Buff")]
public class PlayerBuffActionData : PlayerActionData
{
    [Header("버프"), SerializeField] protected BuffData[] buffDatas;
    public BuffData[] GetBuffData() { return buffDatas; }
}

[Serializable]
public class BuffData
{
    [SerializeField, Tooltip("버프 효과")] protected E_BUFF_APPLY_STATS applyStatType;
    [SerializeField, Tooltip("어떤 스탯이 버프에 효과를 주는지")] protected E_BUFF_EFFECT_STATS effectStatType;
    [SerializeField, Tooltip("버프 지속성")] protected E_BUFF_COUNTINUITIES continuityType;
    [SerializeField, Tooltip("버프 비율")] protected float[] buffMultipliers;

    public E_BUFF_APPLY_STATS GetApplyStatType {  get { return applyStatType; } }
    public E_BUFF_EFFECT_STATS GetEffectStatType { get { return effectStatType; } } 
    public E_BUFF_COUNTINUITIES GetContinuityType { get {return continuityType; } }
    public float GetBuffMultipliers(int _level) {  return buffMultipliers[_level-1]; }
}