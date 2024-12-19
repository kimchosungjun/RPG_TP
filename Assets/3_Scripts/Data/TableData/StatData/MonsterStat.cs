using UnityEngine;
using System;

/// <summary>
/// 비전투 몬스터의 경우 상속받지 않고 사용
/// </summary>
[Serializable]
public class MonsterStat : BaseStat
{
    #region Protected_StatValue 
    [SerializeField] protected int monsterID;
    [SerializeField] protected int monsterLevel;
    [SerializeField] protected float monsterCurHP;
    [SerializeField] protected float boostSpeed;
    #endregion

    #region Property_StatValue
    public int GetMonsterID { get { return monsterID; } }
    public int MonsterLevel { get {return monsterLevel; } set { monsterLevel = value; } } 
    public float MonsterCurHP { get { return monsterCurHP; } set { monsterCurHP = value; } }
    public float BoostSpeed { get { return boostSpeed; }  set { boostSpeed = value; } }

    /// <summary>
    /// 스탯을 초기화할때 호출 (처음 생성시, 혹은 스탯 리셋시)
    /// </summary>
    /// <param name="_nonCombatMonsterStat"></param>
    public void SetMonsterStat(MonsterTableClasses.NonCombatMonsterStatTableData _nonCombatMonsterStat, int _level)
    {
        maxHp = _nonCombatMonsterStat.maxHP;
        monsterCurHP = maxHp;
        speed = _nonCombatMonsterStat.speed;
        defenceValue = _nonCombatMonsterStat.defence;
        monsterID = _nonCombatMonsterStat.ID;    
        monsterLevel = _level;
        boostSpeed = _nonCombatMonsterStat.boostSpeed;
        actorName = SharedMgr.TableMgr.Monster.GetMonsterInfoTableData(monsterID).name;
    }

    #endregion
}