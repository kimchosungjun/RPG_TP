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
    [SerializeField] protected int currentHP;
    [SerializeField] protected float boostSpeed;
    [SerializeField] protected int attackValue;
    [SerializeField] protected float criticalValue;
    #endregion

    #region Property_StatValue
    public int GetID { get { return ID; } }
    public int Level { get {return level; } set { level = value; } } 
    public int CurrentHP { get { return currentHP; } set { currentHP = value; } }
    public float BoostSpeed { get { return boostSpeed; }  set { boostSpeed = value; } }
    public int Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }

    /// <summary>
    /// 스탯을 초기화할때 호출 (처음 생성시, 혹은 스탯 리셋시)
    /// </summary>
    /// <param name="_nonCombatMonsterStat"></param>
    public virtual void SetMonsterStat(MonsterTableClassGroup.MonsterStatTableData _tableData, int _level)
    {
        level = _level;
        int levelDiff = _level - _tableData.startLevel;
        maxHp = _tableData.maxHP + _tableData.hpIncrease * levelDiff;
        currentHP = maxHp;
        speed = _tableData.speed;
        defenceValue = _tableData.defence + _tableData.defenceIncrease * levelDiff;
        ID = _tableData.ID;
        boostSpeed = _tableData.boostSpeed;
        attackValue = _tableData.attack + _tableData.attackIncrease * levelDiff;
        criticalValue = _tableData.critical + _tableData.criticalIncrease * levelDiff;
        actorName = SharedMgr.TableMgr.GetMonster.GetMonsterInfoTableData(ID).name;
    }

    public int GetCurrentHPPercent()
    {
        float result = currentHP *100f / maxHp;
        int res = (int)(result);
        return res;
    }

    public void ResetMonsterStat()
    {
        MonsterTableClassGroup.MonsterStatTableData tableData = SharedMgr.TableMgr.GetMonster.GetMonsterStatTableData(ID);
        int levelDiff = level - tableData.startLevel;
        maxHp = tableData.maxHP + tableData.hpIncrease * levelDiff;
        currentHP = maxHp;
        speed = tableData.speed;
        defenceValue = tableData.defence + tableData.defenceIncrease * levelDiff;
        ID = tableData.ID;
        boostSpeed = tableData.boostSpeed;
        attackValue = tableData.attack + tableData.attackIncrease * levelDiff;
        criticalValue = tableData.critical + tableData.criticalIncrease * levelDiff;
        actorName = SharedMgr.TableMgr.GetMonster.GetMonsterInfoTableData(ID).name;
    }

    #endregion
}