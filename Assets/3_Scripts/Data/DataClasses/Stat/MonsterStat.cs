using UnityEngine;
using System;

/// <summary>
/// 비전투 몬스터의 경우 상속받지 않고 사용
/// </summary>
[Serializable]
public class MonsterStat : BaseStat
{
    #region Protected_StatValue 
    [SerializeField] protected int ID;
    [SerializeField] protected int level;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float boostSpeed;
    [SerializeField] protected float attackValue;
    [SerializeField] protected float criticalValue;
    #endregion

    #region Property_StatValue
    public int GetID { get { return ID; } }
    public int Level { get {return level; } set { level = value; } } 
    public float CurrentHP { get { return currentHP; } set { currentHP = value; } }
    public float BoostSpeed { get { return boostSpeed; }  set { boostSpeed = value; } }
    public float Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }

    /// <summary>
    /// 스탯을 초기화할때 호출 (처음 생성시, 혹은 스탯 리셋시)
    /// </summary>
    /// <param name="_nonCombatMonsterStat"></param>
    public virtual void SetMonsterStat(MonsterTableClasses.MonsterStatTableData _combatMonsterStat, int _level)
    {
        level = _level;
        int levelDiff = _level - _combatMonsterStat.startLevel;
        maxHp = _combatMonsterStat.maxHP + _combatMonsterStat.hpIncrease * levelDiff;
        currentHP = maxHp;
        speed = _combatMonsterStat.speed;
        defenceValue = _combatMonsterStat.defence + _combatMonsterStat.defenceIncrease * levelDiff;
        ID = _combatMonsterStat.ID;
        boostSpeed = _combatMonsterStat.boostSpeed;
        attackValue = _combatMonsterStat.attack + _combatMonsterStat.attackIncrease * levelDiff;
        criticalValue = _combatMonsterStat.critical + _combatMonsterStat.criticalIncrease * levelDiff;
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(ID).name;
    }

    #endregion
}