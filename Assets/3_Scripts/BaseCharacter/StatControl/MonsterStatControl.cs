using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : ActorStatControl
{
    MonsterStat monsterStat = null;
    BaseMonster baseMonster = null;
    
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    public void SetBaseMonster(BaseMonster _baseMonster) { this.baseMonster = _baseMonster; }
    public override void Heal(float _heal)
    {
        base.Heal(_heal);
        throw new System.NotImplementedException();
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        base.Recovery(_percent, _time); 
        // 현재 체력과 최대체력 비교
        // monsterStat.HP = 
    }

    IEnumerator CRecovery(float _percent)
    {
        yield return null;  
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        // Min Damage = 1
        float allDamage = _attackData.GetAttackValue - monsterStat.Defence;
        allDamage = (allDamage) <= 0 ? 1 : allDamage;
        // Float Damage
        SharedMgr.PoolMgr.GetFloatDamageText().SetFloat(this.transform, allDamage);
        // Check Death
        float curHp = monsterStat.CurrentHP - allDamage;
        if (curHp <= 0)
        {
            monsterStat.CurrentHP = 0f;
            baseMonster.Death();
        }
        else
        {
            monsterStat.CurrentHP = curHp;
        }
        // 데미지를 스탯에 적용 후 UI에 표기
        base.TakeDamage(_attackData);
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
        float conditionValue = _conditionData.GetConditionValue;
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
                monsterStat.Attack += conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                monsterStat.Defence += conditionValue;
                break;
            default :
                // monster dosen't have attack speed stat
                break;
        }
    }

    public override void DeleteConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = _conditionData.GetConditionValue;
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
                monsterStat.Attack -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                monsterStat.Defence -= conditionValue;
                break;
            default:
                // monster dosen't have attack speed stat
                break;
        }
    }
    #endregion
}
