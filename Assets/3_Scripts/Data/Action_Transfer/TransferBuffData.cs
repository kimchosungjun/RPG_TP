using EffectEnums;
using UnityEngine;

public class TransferBuffData
{
    [SerializeField] protected BUFF_APPLY_STATS buffStat;
    [SerializeField] protected BUFF_ATTRIBUTE_STATS useStat;
    [SerializeField] protected BUFF_TYPES continuity;
    [SerializeField] protected float buffValue;
    [SerializeField] protected float buffTime;
    [SerializeField] protected float doBuffTime;

    public BUFF_APPLY_STATS GetBuffStatType { get { return buffStat; } }
    public BUFF_ATTRIBUTE_STATS GetUseStatType { get { return useStat; } }
    public BUFF_TYPES GetBuffContinuity { get { return continuity; } }  

    public TransferBuffData() { }

    public void SetData(int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime)
    {
        this.buffStat = (BUFF_APPLY_STATS)_buffStat;
        this.useStat = (BUFF_ATTRIBUTE_STATS)_useStat;
        this.continuity = (BUFF_TYPES)_buffContinuity;
        this.buffValue = _buffValue;
        this.buffTime = _buffTime;
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
