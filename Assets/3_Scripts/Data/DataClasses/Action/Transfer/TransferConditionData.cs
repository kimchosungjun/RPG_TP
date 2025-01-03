using EffectEnums;
using UnityEngine;
using UnityEngine.Events;

public class TransferConditionData
{
    #region Value
    [SerializeField] protected CONDITION_EFFECT_STATS conditionStat;
    [SerializeField] protected CONDITION_CONTINUITY conditionContinuity;
    [SerializeField] protected float conditionValue;
    [SerializeField] protected float maxConditionTime;
    [SerializeField] protected float effectConditionTime = 0f;
    [SerializeField] bool isEndConditionTime = false;

    public CONDITION_EFFECT_STATS GetConditionStat { get { return conditionStat; } }
    public CONDITION_CONTINUITY GetConditionContinuity { get { return conditionContinuity; } }
    public float GetConditionValue { get { return conditionValue; } }
    public bool GetIsEndConditionTime { get { return isEndConditionTime; } }
    #endregion

    #region Creator
    public TransferConditionData() { }
    #endregion

    #region Set Transfer Data
    public void SetData(PlayerStat _playerStat, int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime, float _multiplier)
    {
        this.conditionStat = (CONDITION_EFFECT_STATS)_buffStat;
        this.conditionContinuity = (CONDITION_CONTINUITY)_buffContinuity;
        this.maxConditionTime = _buffTime;
        this.effectConditionTime = 0f;

        CONDITION_ATTRIBUTE_STATS attribute = (CONDITION_ATTRIBUTE_STATS)_useStat;
        float statValue = 0f;
        switch (attribute)
        {
            case CONDITION_ATTRIBUTE_STATS.NONE:
                break;
            case CONDITION_ATTRIBUTE_STATS.MAXHP:
                statValue = _playerStat.MaxHP;
                break;
            case CONDITION_ATTRIBUTE_STATS.ATK:
                statValue = _playerStat.Attack;
                break;
            case CONDITION_ATTRIBUTE_STATS.DEF:
                statValue = _playerStat.Defence;
                break;
        }
        this.conditionValue = _buffValue + statValue*_multiplier;
    }

    public void SetData(MonsterStat _monsterStat, int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime, float _multiplier)
    {
        this.conditionStat = (CONDITION_EFFECT_STATS)_buffStat;
        this.conditionContinuity = (CONDITION_CONTINUITY)_buffContinuity;
        this.maxConditionTime = _buffTime;
        this.effectConditionTime = 0f;

        CONDITION_ATTRIBUTE_STATS attribute = (CONDITION_ATTRIBUTE_STATS)_useStat;
        float statValue = 0f;
        switch (attribute)
        {
            case CONDITION_ATTRIBUTE_STATS.NONE:
                break;
            case CONDITION_ATTRIBUTE_STATS.MAXHP:
                statValue = _monsterStat.MaxHP;
                break;
            case CONDITION_ATTRIBUTE_STATS.ATK:
                statValue = _monsterStat.Attack;
                break;
            case CONDITION_ATTRIBUTE_STATS.DEF:
                statValue = _monsterStat.Defence;
                break;
        }
        this.conditionValue = _buffValue + statValue * _multiplier;
    }
    #endregion

    #region Manage Condition Stat

    public void UpdateConditionTime()
    {
        if (isEndConditionTime == true)
            return;
        effectConditionTime += Time.fixedDeltaTime;
        isEndConditionTime = (effectConditionTime>=maxConditionTime) ? true : false;
    }
    #endregion
}
