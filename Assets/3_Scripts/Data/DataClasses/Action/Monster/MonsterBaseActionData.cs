using UnityEngine;
using System;

[Serializable]
public class MonsterBaseActionData 
{
    [SerializeField] protected int attribute;
    [SerializeField] protected float multiplier;
    [SerializeField] protected int effect;
    [SerializeField] protected float maintainTime;
    [SerializeField] protected float coolTime;
    [SerializeField] protected float defaultValue;

    public int GetAttribute { get { return attribute; }}
    public float GetMultiplier { get { return multiplier; }}
    public int GetEffect { get { return effect; }}  
    public float GetMaintainTime { get { return maintainTime; }}
    public float GetCoolTime { get { return coolTime; }}    
    public float GetDefaultValue { get { return defaultValue; }}
}
