using System;
using UnityEngine;

public class EliteMonsterStat : CombatMonsterStat
{
    float groggyValue = 100f;

    public float GroggyValue { get { return groggyValue; } set { groggyValue = value; } }
}