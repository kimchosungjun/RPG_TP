using EffectEnums;
using UnityEngine;

public class BuffData
{
    public BUFF_APPLY_STATS buffStat;
    public BUFF_USE_STATS useStat;
    public BUFF_COUNTINUITIES continuity;
    public float buffValue;
    public float buffTime;
    public float doBuffTime;

    public BuffData(int _buffStat, int _useStat, int _buffContinuity, float _buffValue, float _buffTime)
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
