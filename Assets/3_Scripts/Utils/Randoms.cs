using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randoms
{
    public static int GetRandomCnt(int _inclue_start, int _exclude_end)
    {
        int randNum = Random.Range(_inclue_start, _exclude_end);
        return randNum;
    }

    public static float GetCritical(float _criticalPercent)
    {
        float randFloat = Random.value;
        return (randFloat <= _criticalPercent) ? 1.5f : 1f;
    }

    public static float GetCritical(float _criticalPercent, ref bool _isCritical)
    {
        float randFloat = Random.value;
        if(randFloat <= _criticalPercent)
        {
            _isCritical = true;
            return 1.5f;
        }
        else
        {
            _isCritical = false;
            return 1f;
        }
    }
    
    public static bool IsInProbability(float _percent)
    {
        return Random.value < _percent;
    }
}
