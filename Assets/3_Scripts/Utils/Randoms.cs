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
    
    public static void IsInProbability(float[] _probabilities, ref int _index)
    {
        float randValue = Random.value;
        float probability = 0.0f;
        int cnt = _probabilities.Length;
        for (int i = 0; i < cnt; i++)
        {
            probability += _probabilities[i];
            if (probability < randValue)
            {
               _index = i;
                return;
            }
        }
        _index = 0;
    }

    public static void GetQuantity(int _startQuantity, int _endQuantity, float[] _probabilities, ref int _quantity)
    {
        if(_startQuantity == _endQuantity) { _quantity = _startQuantity; return; }

        int cnt= _endQuantity - _startQuantity + 1;
        int[] quantitySet = new int[cnt];
        for (int i = 0; i < cnt; i++)
        {
            quantitySet[i] = _startQuantity + i;
        }

        float probability = 0.0f;
        float randValue = Random.value;
        for(int i=0; i<cnt; i++)
        {
            probability += _probabilities[i];
            if (probability < randValue)
            {
                _quantity = quantitySet[i];
                return;
            }
        }
        _quantity = 1;
    }
}
