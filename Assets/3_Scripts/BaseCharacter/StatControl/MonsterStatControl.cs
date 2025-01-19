using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : ActorStatControl
{
    MonsterStat monsterStat = null;
    BaseMonster baseMonster = null;
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    public BaseMonster BaseMonster { get { return baseMonster; } set { baseMonster = value; } } 
    
    public bool CheckFullHp()
    {
        if (monsterStat.CurrentHP == monsterStat.MaxHP) return true;
        return false;
    }

    public override void Heal(float _heal, bool _isPercent = false)
    {
        float increaseHP = 0f;
        if (monsterStat.CurrentHP <=0) return; // Death State
        if (_isPercent)
            increaseHP = monsterStat.MaxHP * _heal;
        else
            increaseHP = monsterStat.CurrentHP + _heal;

        monsterStat.CurrentHP = increaseHP > monsterStat.MaxHP
                ? monsterStat.MaxHP : (int)increaseHP;
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        // Min Damage = 1
        int allDamage = _attackData.GetAttackValue - monsterStat.Defence;
        allDamage = (allDamage) <= 0 ? 1 : allDamage;
        // Float Damage
        SharedMgr.PoolMgr.GetFloatDamageText().SetFloat(this.transform, allDamage);
        // Check Death
        int curHp = monsterStat.CurrentHP - (int)allDamage;
        if (curHp <= 0)
        {
            monsterStat.CurrentHP = 0;
            baseMonster.Death();
        }
        else
        {
            monsterStat.CurrentHP = curHp;
        }
    }

    /// <summary>
    /// Call When Monster Respawn
    /// </summary>
    public void ResetStat()
    {

    }

    #region Apply Condition Data
    public override void ApplyConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = _conditionData.ConditionValue;
        switch (_conditionData.GetConditionContinuity)
        {
            case EffectEnums.CONDITION_CONTINUITY.DEBUFF:
                conditionValue *= -1; 
                break;
            default:
                break;
        }

        switch (_conditionData.GetConditionStat)
        {
            case EffectEnums.CONDITION_EFFECT_STATS.HP:
                Heal(conditionValue);
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.SPD:
                monsterStat.Speed += conditionValue;
                monsterStat.BoostSpeed += conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                monsterStat.Attack += (int)conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                monsterStat.Defence += (int)conditionValue;
                break;
            default :
                // monster dosen't have attack speed stat
                break;
        }
    }

    public override void DeleteConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = _conditionData.ConditionValue;
        switch (_conditionData.GetConditionContinuity)
        {
            case EffectEnums.CONDITION_CONTINUITY.DEBUFF:
                conditionValue *= -1;
                break;
            default:
                break;
        }

        switch (_conditionData.GetConditionStat)
        {
            case EffectEnums.CONDITION_EFFECT_STATS.SPD:
                monsterStat.Speed -= conditionValue;
                monsterStat.BoostSpeed -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                monsterStat.Attack -= (int)conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                monsterStat.Defence -= (int)conditionValue;
                break;
            default:
                // monster dosen't have attack speed stat
                break;
        }
    }
    #endregion
}
