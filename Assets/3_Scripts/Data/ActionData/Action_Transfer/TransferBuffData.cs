using EffectEnums;
using UnityEngine;

public class TransferBuffData
{
    [SerializeField] protected BUFF_APPLY_STATS buffStat;
    [SerializeField] protected BUFF_USE_STATS useStat;
    [SerializeField] protected BUFF_COUNTINUITIES continuity;
    [SerializeField] protected float buffValue;
    [SerializeField] protected float buffTime;
    [SerializeField] protected float doBuffTime;

    public BUFF_APPLY_STATS GetBuffStatType { get { return buffStat; } }
    public BUFF_USE_STATS GetUseStatType { get { return useStat; } }
    public BUFF_COUNTINUITIES GetBuffContinuity { get { return continuity; } }  

    public TransferBuffData() { }

    public void SetData(int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime)
    {
        this.buffStat = (BUFF_APPLY_STATS)_buffStat;
        this.useStat = (BUFF_USE_STATS)_useStat;
        this.continuity = (BUFF_COUNTINUITIES)_buffContinuity;
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
