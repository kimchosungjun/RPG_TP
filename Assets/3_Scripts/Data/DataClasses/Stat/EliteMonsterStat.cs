using MonsterTableClasses;
using System;
using UnityEngine;

public class EliteMonsterStat : MonsterStat
{
    public override void SetMonsterStat(MonsterStatTableData _monsterStat, int _level)
    {
        base.SetMonsterStat(_monsterStat, _level);
        groggyValue = 100f;
    }

    float groggyValue = 100f;
    public float GroggyValue { get { return groggyValue; } set { groggyValue = value; BeGroggyState(); } }

    public void GetCritical() 
    {
        groggyValue = (groggyValue - 10f > 0f)? groggyValue-10f : 0;
    }

    public bool BeGroggyState()
    {
        if (groggyValue <= 0.1f)
        {
            // To Do ~~~~~ 
            // Anounce Groggy State
            return true;
        }
        return false;
    }
}