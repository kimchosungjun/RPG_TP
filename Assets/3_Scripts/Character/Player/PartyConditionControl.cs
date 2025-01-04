using EffectEnums;
using System.Collections.Generic;
using UnityEngine;

public class PartyConditionControl : PlayerStatControl
{
    [SerializeField] PlayerCtrl playerCtrl;

    /******************************************/
    /*********** 플레이어 변경 *************/
    /******************************************/

    #region Player Ctrl : Stat Control
    public void ChangePlayer(PlayerStat _nextPlayer)
    {
        if (buffCnt != 0)
        {
            // Remove Legacy Player Condition
            for (int i = 0; i <buffCnt ; i++)
            {
                DeleteConditionData(currentConditions[i]);
            }

            // Add Next Player Condition
            playerStat = _nextPlayer;
            for (int i = 0; i < buffCnt; i++)
            {
                ApplyConditionData(currentConditions[i]);
            }
        }
    }

    //  + Dash Gauge
    public override void ApplyConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = _conditionData.GetConditionValue;
        switch (_conditionData.GetConditionContinuity)
        {
            case CONDITION_CONTINUITY.DEBUFF:
                conditionValue *= -1;
                break;
            default:
                break;
        }

        switch (_conditionData.GetConditionStat)
        {
            case CONDITION_EFFECT_STATS.HP:
                Heal(conditionValue);
                break;
            case CONDITION_EFFECT_STATS.SPD:
                playerStat.Speed += conditionValue;
                break;
            case CONDITION_EFFECT_STATS.ATK:
                playerStat.Attack += conditionValue;
                break;
            case CONDITION_EFFECT_STATS.DEF:
                playerStat.Defence += conditionValue;
                break;
            case CONDITION_EFFECT_STATS.ATKSPD:
                playerStat.AttackSpeed += conditionValue;
                break;
            case CONDITION_EFFECT_STATS.DASH:
                playerCtrl.DecreaseDashGauge(conditionValue);
                break;
            default:
                break;
        }
    }
    #endregion
}
