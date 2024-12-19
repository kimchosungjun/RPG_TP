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
    #endregion

    #region Property_StatValue
    public int GetID { get { return ID; } }
    public int Level { get {return level; } set { level = value; } } 
    public float CurrentHP { get { return currentHP; } set { currentHP = value; } }
    public float BoostSpeed { get { return boostSpeed; }  set { boostSpeed = value; } }

    /// <summary>
    /// 스탯을 초기화할때 호출 (처음 생성시, 혹은 스탯 리셋시)
    /// </summary>
    /// <param name="_nonCombatMonsterStat"></param>
    public void SetMonsterStat(MonsterTableClasses.NonCombatMonsterStatTableData _nonCombatMonsterStat, int _level)
    {
        int levelDiff = _level - _nonCombatMonsterStat.startLevel;
        maxHp = _nonCombatMonsterStat.maxHP + _nonCombatMonsterStat.hpIncrease*levelDiff;
        currentHP = maxHp;
        speed = _nonCombatMonsterStat.speed;
        defenceValue = _nonCombatMonsterStat.defence + _nonCombatMonsterStat.defenceIncrease*levelDiff;
        ID = _nonCombatMonsterStat.ID;    
        level = _level;
        boostSpeed = _nonCombatMonsterStat.boostSpeed;
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(ID).name;
    }

    #endregion
}