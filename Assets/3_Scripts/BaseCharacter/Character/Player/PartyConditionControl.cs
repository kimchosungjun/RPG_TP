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
    public void SetPlayerStat(PlayerStat _playerStat)
    {
        playerStat = _playerStat;
    }

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
            return;
        }
        playerStat = _nextPlayer;
    }

    //  + Dash Gauge
    public override void ApplyConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = 0f;
        switch (_conditionData.GetConditionContinuity)
        {
            case CONDITION_CONTINUITY.DEBUFF:
                conditionValue *= -1;
                break;
            default:
                break;
        }
        switch (_conditionData.GetConditionApplyType)
        {
            #region Value
            case CONDITION_APPLY_TYPE.VALUE:
                conditionValue = Mathf.Round(_conditionData.ConditionValue);
                switch (_conditionData.GetConditionStat)
                {
                    case CONDITION_EFFECT_STATS.HP:
                        PartyHeal(conditionValue);
                        break;
                    case CONDITION_EFFECT_STATS.SPD:
                        playerStat.Speed += conditionValue;
                        break;
                    case CONDITION_EFFECT_STATS.ATK:
                        playerStat.Attack += _conditionData.GetConditionIntValue();
                        break;
                    case CONDITION_EFFECT_STATS.DEF:
                        playerStat.Defence += _conditionData.GetConditionIntValue();
                        break;
                    case CONDITION_EFFECT_STATS.ATKSPD:
                        playerStat.AttackSpeed += conditionValue;
                        break;
                    default:
                        break;
                }
                break;
            #endregion
            #region Percent
            case CONDITION_APPLY_TYPE.OWN_PERCENT:
                conditionValue = _conditionData.GetMuliplier;
                switch (_conditionData.GetConditionStat)
                {
                    case CONDITION_EFFECT_STATS.HP:
                        PartyHeal(conditionValue, true);
                        _conditionData.ConditionValue = 0f;
                        break;
                    case CONDITION_EFFECT_STATS.SPD:
                        float spdValue = conditionValue * playerStat.Speed;
                        playerStat.Speed += spdValue;
                        _conditionData.ConditionValue = spdValue;
                        break;
                    case CONDITION_EFFECT_STATS.ATK:
                        int atkValue = (int)(Mathf.Round(conditionValue * playerStat.Attack));
                        playerStat.Attack += atkValue;
                        _conditionData.ConditionValue = atkValue;
                        break;
                    case CONDITION_EFFECT_STATS.DEF:
                        int defValue = (int)(Mathf.Round(conditionValue * playerStat.Defence));
                        playerStat.Defence += defValue;
                        _conditionData.ConditionValue = defValue;
                        break;
                    case CONDITION_EFFECT_STATS.ATKSPD:
                        float atkSpdValue = conditionValue * playerStat.AttackSpeed;
                        playerStat.AttackSpeed += atkSpdValue;
                        _conditionData.ConditionValue = atkSpdValue;
                        break;
                    case CONDITION_EFFECT_STATS.DASH:
                        playerCtrl.DecreaseDashGauge(conditionValue);
                        break;
                    default:
                        break;
                }
                break;
            #endregion
            default:
                break;
        }
    }

    public void AddAllPartyExp(int _exp)
    {
        List<BasePlayer> players = playerCtrl.GetPlayers;
        if (players == null) return;

        int playerCnt = players.Count;  
        for(int i=0; i<playerCnt; i++)
        {
            players[i].GetPlayerStatControl.GetExp(_exp);
        }
    }

    public void PartyHeal(float _heal, bool _isPercent = false)
    {
        List< BasePlayer> players = playerCtrl.GetPlayers;
        int playerCnt = players.Count;
        for(int i=0; i<playerCnt; i++)
        {
            players[i].GetPlayerStatControl.Heal(_heal);    
        }
    }
    #endregion
}
