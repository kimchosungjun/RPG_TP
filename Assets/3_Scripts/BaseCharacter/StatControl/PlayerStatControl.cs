using PlayerTableClasses;
using System;
using UnityEngine;

[Serializable]
public class PlayerStatControl : ActorStatControl
{
    [SerializeField] protected PlayerStat playerStat = null;
    protected BasePlayer player = null;
    public PlayerStat PlayerStat { get { return playerStat; } set { playerStat = value; } }
    public BasePlayer Player { get { return player; } set { player = value; } }

    #region Hp

    public override void Heal(float _heal, bool _isPercent = false)
    {
        float increaseHP = 0f;
        if (CheckDeathState()) return; // Death State
        if (_isPercent)
            increaseHP = playerStat.MaxHP * _heal;
        else
            increaseHP = playerStat.GetSaveStat.currentHP + _heal;

        playerStat.GetSaveStat.currentHP = increaseHP > playerStat.MaxHP
                ? playerStat.MaxHP : (int)increaseHP;
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        playerStat.GetSaveStat.currentHP = playerStat.MaxHP;
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        int allDamage = _attackData.GetAttackValue - playerStat.Defence;
        allDamage = (allDamage <= 0) ? 1 : allDamage;
        playerStat.GetSaveStat.currentHP -= allDamage;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.UpdateData(playerStat);
        if (playerStat.GetSaveStat.currentHP <= 0) Death();
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

    #region Exp : To Do Link UI
    public void GetExp(int _exp)
    {
        PlayerSaveStat saveStat = playerStat.GetSaveStat;
        PlayerLevelTableData levelTableData = SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData();
        int currentLevel = saveStat.currentLevel;
        int maxLevel = levelTableData.maxLevel;
        int currentMaxExp = levelTableData.needExps[currentLevel - 1];
        int exp = _exp;
        if (currentLevel == maxLevel)
            return;

        for (; ; )
        {
            if (exp + saveStat.currentExp < currentMaxExp)
            {
                saveStat.currentExp += exp;
                return;
            }
            else
            {
                saveStat.currentExp = 0;
                saveStat.currentLevel += 1;
                exp = currentMaxExp - saveStat.currentExp;
                currentLevel += 1;

                if (currentLevel == maxLevel || exp == 0)
                    return;

                currentMaxExp = levelTableData.needExps[currentLevel - 1];
            }
        }
    }

    #endregion

    #region Apply Condition Data
    public override void ApplyConditionData(TransferConditionData _conditionData)
    {
        float conditionValue = 0f;
        switch (_conditionData.GetConditionContinuity)
        {
            case EffectEnums.CONDITION_CONTINUITY.DEBUFF:
                conditionValue *= -1;
                break;
            default:
                break;
        }
        switch (_conditionData.GetConditionApplyType)
        {
            #region Value
            case EffectEnums.CONDITION_APPLY_TYPE.VALUE:
                conditionValue = MathF.Round(_conditionData.ConditionValue);
                switch (_conditionData.GetConditionStat)
                {
                    case EffectEnums.CONDITION_EFFECT_STATS.HP:
                        Heal(conditionValue);
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.SPD:
                        playerStat.Speed += conditionValue;
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                        playerStat.Attack += _conditionData.GetConditionIntValue();
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                        playerStat.Defence += _conditionData.GetConditionIntValue();
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.ATKSPD:
                        playerStat.AttackSpeed += conditionValue;
                        break;
                    default:
                        break;
                }
                break;
            #endregion
            #region Percent
            case EffectEnums.CONDITION_APPLY_TYPE.OWN_PERCENT:
                conditionValue = _conditionData.GetMuliplier;
                switch (_conditionData.GetConditionStat)
                {
                    case EffectEnums.CONDITION_EFFECT_STATS.HP:
                        Heal(conditionValue, true);
                        _conditionData.ConditionValue = 0f;
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.SPD:
                        float spdValue = conditionValue * playerStat.Speed;
                        playerStat.Speed += spdValue;
                        _conditionData.ConditionValue = spdValue;
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                        int atkValue = (int)(Mathf.Round(conditionValue * playerStat.Attack));
                        playerStat.Attack += atkValue;
                        _conditionData.ConditionValue = atkValue;
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                        int defValue = (int)(Mathf.Round(conditionValue * playerStat.Defence));
                        playerStat.Defence += defValue;
                        _conditionData.ConditionValue = defValue;
                        break;
                    case EffectEnums.CONDITION_EFFECT_STATS.ATKSPD:
                        float atkSpdValue = conditionValue* playerStat.AttackSpeed;
                        playerStat.AttackSpeed += atkSpdValue;
                        _conditionData.ConditionValue = atkSpdValue;
                        break;
                    default:
                        break;
                }
                break;
            #endregion
            default: break;
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
                playerStat.Speed -= conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.ATK:
                playerStat.Attack -= (int)conditionValue;
                break;
            case EffectEnums.CONDITION_EFFECT_STATS.DEF:
                playerStat.Defence -= (int)conditionValue;
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
