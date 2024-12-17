using UnityEngine;
using System;

[Serializable]
public class CombatMonsterStat : MonsterStat
{
    [SerializeField] protected float attackValue;
    [SerializeField] protected float criticalValue;
    public float Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }

    public void SetMonsterStat(MonsterTableClasses.CombatMonsterStatTableData _combatMonsterStat)
    {
        maxHp = _combatMonsterStat.monsterMaxHP;
        speed = _combatMonsterStat.monsterSpeed;
        defenceValue = _combatMonsterStat.monsterDefence;
        monsterID = _combatMonsterStat.monsterID;
        monsterLevel = _combatMonsterStat.monsterLevel;
        boostSpeed = _combatMonsterStat.monsterBoostSpeed;
        attackValue = _combatMonsterStat.monsterAttack;
        criticalValue = _combatMonsterStat.monsterCritical; 
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(monsterID).monsterName;
    }
}
