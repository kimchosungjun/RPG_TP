using UnityEngine;
using System;

[Serializable]
public class CombatMonsterStat : MonsterStat
{
    [SerializeField] protected float attackValue;
    [SerializeField] protected float criticalValue;
    public float Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }

}
