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
}
