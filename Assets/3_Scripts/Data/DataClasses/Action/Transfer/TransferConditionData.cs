using EffectEnums;
using UnityEngine;
using UnityEngine.Events;

public class TransferConditionData
{
    [SerializeField] protected CONDITION_EFFECT_STATS conditionStat;
    [SerializeField] protected CONDITION_CONTINUITY continuity;
    [SerializeField] protected float conditionValue;
    [SerializeField] protected float maxConditionTime;
    [SerializeField] protected float effectConditionTime = 0f;
    
    bool isEndConditionTime = false;
    public bool GetIsEndConditionTime { get { return isEndConditionTime; } }
    public CONDITION_EFFECT_STATS GetConditionStatType { get { return conditionStat; } }
    public CONDITION_CONTINUITY GetConditionContinuity { get { return continuity; } }  

    public TransferConditionData() { }

    public void SetData(PlayerStat _playerStat, int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime, float _multiplier)
    {
        this.conditionStat = (CONDITION_EFFECT_STATS)_buffStat;
        this.continuity = (CONDITION_CONTINUITY)_buffContinuity;
        this.maxConditionTime = _buffTime;

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
        this.continuity = (CONDITION_CONTINUITY)_buffContinuity;
        this.maxConditionTime = _buffTime;

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

    public void UpdateStatData()
    {

    }

    public void AddBuff()
    {
        effectConditionTime = 0f;

        switch (conditionStat)
        {
            case CONDITION_EFFECT_STATS.HP:
                
                break;
            case CONDITION_EFFECT_STATS.SPD:
                break;
            case CONDITION_EFFECT_STATS.ATK:
                break;
            case CONDITION_EFFECT_STATS.DEF:
                break;
            case CONDITION_EFFECT_STATS.ATKSPD:
                break;
        }
        //if (_playerBuff != null)
        //    _playerBuff.Invoke();
    }

    public void CheckConditionTime()
    {
        if (isEndConditionTime == true)
            return;
        effectConditionTime += Time.fixedDeltaTime;
        isEndConditionTime = (effectConditionTime>=maxConditionTime) ? true : false;
    }

    public void DeleteBuff()
    {

    }
}
