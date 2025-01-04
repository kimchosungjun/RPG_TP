using EffectEnums;
using System;
using UnityEngine;

[Serializable]
public class TransferAttackData 
{
    [SerializeField] protected HIT_EFFECTS hitEffectType;
    [SerializeField] protected int attackValue;
    [SerializeField] protected float effectMaintainTime = 0f;

    public HIT_EFFECTS GetHitEffect { get { return hitEffectType; } }
    public int GetAttackValue { get { return attackValue; } } 
    public float EffectMaintainTime { get { return effectMaintainTime; } set { effectMaintainTime = value; } }

    public TransferAttackData() { }

    public void SetData(int _hitEffectType, float _attackValue, float _effectMaintainTime)
    {
        this.hitEffectType = (HIT_EFFECTS)_hitEffectType;
        this.attackValue = (int)(Mathf.Round(_attackValue));
        this.effectMaintainTime = _effectMaintainTime;
    }   
}
