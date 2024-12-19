using UnityEngine;
using System;

[Serializable]
public class CombatMonsterStat : MonsterStat
{
    [SerializeField] protected float attackValue;
    [SerializeField] protected float criticalValue;
    public float Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }

    public void SetMonsterStat(MonsterTableClasses.CombatMonsterStatTableData _combatMonsterStat, int _level)
    {
        level = _level;
        int levelDiff = _level - _combatMonsterStat.startLevel;
        maxHp = _combatMonsterStat.maxHP + _combatMonsterStat.hpIncrease*levelDiff;
        currentHP = maxHp;
        speed = _combatMonsterStat.speed;
        defenceValue = _combatMonsterStat.defence + _combatMonsterStat.defenceIncrease*levelDiff;
        ID = _combatMonsterStat.ID;
        boostSpeed = _combatMonsterStat.boostSpeed;
        attackValue = _combatMonsterStat.attack + _combatMonsterStat.attackIncrease *levelDiff;
        criticalValue = _combatMonsterStat.critical + _combatMonsterStat.criticalIncrease * levelDiff; 
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(ID).name;
    }
}
