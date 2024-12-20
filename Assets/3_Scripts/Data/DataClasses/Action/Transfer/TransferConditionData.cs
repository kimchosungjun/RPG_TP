using EffectEnums;
using UnityEngine;

public class TransferConditionData
{
    [SerializeField] protected BUFF_APPLY_STATS buffStat;
    [SerializeField] protected BUFF_TYPES continuity;
    [SerializeField] protected float buffValue;
    [SerializeField] protected float buffTime;
    [SerializeField] protected float doBuffTime;

    public BUFF_APPLY_STATS GetBuffStatType { get { return buffStat; } }
    public BUFF_TYPES GetBuffContinuity { get { return continuity; } }  

    public TransferConditionData() { }

    public void SetData(PlayerStat _playerStat, int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime, float _multiplier)
    {
        this.buffStat = (BUFF_APPLY_STATS)_buffStat;
        this.continuity = (BUFF_TYPES)_buffContinuity;
        this.buffTime = _buffTime;

        BUFF_ATTRIBUTE_STATS attribute = (BUFF_ATTRIBUTE_STATS)_useStat;
        float statValue = 0f;
        switch (attribute)
        {
            case BUFF_ATTRIBUTE_STATS.NONE:
                break;
            case BUFF_ATTRIBUTE_STATS.MAXHP:
                statValue = _playerStat.MaxHP;
                break;
            case BUFF_ATTRIBUTE_STATS.ATK:
                statValue = _playerStat.Attack;
                break;
            case BUFF_ATTRIBUTE_STATS.DEF:
                statValue = _playerStat.Defence;
                break;
        }

        this.buffValue = _buffValue + statValue*_multiplier;
    }

    public void SetData(CombatMonsterStat _monsterStat, int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime, float _multiplier)
    {
        this.buffStat = (BUFF_APPLY_STATS)_buffStat;
        this.continuity = (BUFF_TYPES)_buffContinuity;
        this.buffTime = _buffTime;

        BUFF_ATTRIBUTE_STATS attribute = (BUFF_ATTRIBUTE_STATS)_useStat;
        float statValue = 0f;
        switch (attribute)
        {
            case BUFF_ATTRIBUTE_STATS.NONE:
                break;
            case BUFF_ATTRIBUTE_STATS.MAXHP:
                statValue = _monsterStat.MaxHP;
                break;
            case BUFF_ATTRIBUTE_STATS.ATK:
                statValue = _monsterStat.Attack;
                break;
            case BUFF_ATTRIBUTE_STATS.DEF:
                statValue = _monsterStat.Defence;
                break;
        }

        this.buffValue = _buffValue + statValue * _multiplier;
    }

    public void OverlapBuff()
    {
        doBuffTime = Time.time;
    }

    public void AddBuff()
    {
        doBuffTime = Time.time;
        // To Do ~~~~~
        // Add Buff Stat
    }

    public void UpdateData()
    {

    }

    public void DeleteBuff()
    {

    }

    public bool IsMaintainBuff()
    {
        float curTime = Time.time;
        if (curTime - doBuffTime > buffTime)
            return false;
        return true;
    }
}
