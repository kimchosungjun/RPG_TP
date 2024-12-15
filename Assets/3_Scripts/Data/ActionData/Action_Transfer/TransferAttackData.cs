using EffectEnums;
using UnityEngine;

public class TransferAttackData 
{
    [SerializeField] protected HIT_EFFECTS hitEffectType;
    [SerializeField] protected float attackValue;
    [SerializeField] protected float effectMaintainTime = 0f;

    public HIT_EFFECTS GetHitEffect { get { return hitEffectType; } }
    public float GetAttackValue { get { return attackValue; } } 
    public float EffectMaintainTime { get { return effectMaintainTime; } set { effectMaintainTime = value; } }

    public TransferAttackData() { }

    public void SetData(int _hitEffectType, float _attackValue, float _effectMaintainTime)
    {
        this.hitEffectType = (HIT_EFFECTS)_hitEffectType;
        this.attackValue = _attackValue;
        this.effectMaintainTime = _effectMaintainTime;
    }   
}
