using MonsterTableClasses;
using System;
using UnityEngine;

public class EliteMonsterStat : CombatMonsterStat
{
    public override void SetMonsterStat(CombatMonsterStatTableData _combatMonsterStat, int _level)
    {
        base.SetMonsterStat(_combatMonsterStat, _level);
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