using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatControl : ActorStatControl
{
    protected PlayerStat playerStat = null;
    protected BasePlayer player = null;
    public PlayerStat PlayerStat { get { return playerStat; } set { playerStat = value; } }
    public BasePlayer Player { get { return player;  } set { player = value; } }    
    #region To Do ~~~~~~

   

    public override void Heal(float _heal)
    {

    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        playerStat.GetSaveStat.currentHP -= (_attackData.GetAttackValue-playerStat.Defence);
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.UpdateData(playerStat);    
        if(playerStat.GetSaveStat.currentHP<=0.01f) Death();    
    }
    #endregion

    #region Death
    public override void Death() { player.DoDeathState(); }
    public bool CheckDeathState()
    {
        if (playerStat.GetSaveStat.currentHP <= 0.01f) return true;
        return false;
    }
    #endregion

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
                playerStat.Speed += conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                playerStat.Attack += conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                playerStat.Defence += conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATKSPD:
                playerStat.AttackSpeed += conditionValue;   
                break;
            default:
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
                playerStat.Speed -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                playerStat.Attack -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                playerStat.Defence -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATKSPD:
                playerStat.AttackSpeed -= conditionValue;
                break;
            default:
                break;
        }
    }

    #endregion
}
