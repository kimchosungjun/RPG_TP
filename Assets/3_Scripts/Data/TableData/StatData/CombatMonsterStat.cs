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
        maxHp = _combatMonsterStat.maxHP;
        speed = _combatMonsterStat.speed;
        defenceValue = _combatMonsterStat.defence;
        monsterID = _combatMonsterStat.ID;
        monsterLevel = _level;
        boostSpeed = _combatMonsterStat.boostSpeed;
        attackValue = _combatMonsterStat.attack;
        criticalValue = _combatMonsterStat.critical; 
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(monsterID).name;
    }
}
