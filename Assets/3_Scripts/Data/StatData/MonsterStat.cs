using UnityEngine;

/// <summary>
/// 비전투 몬스터의 경우 바로 상속받아 사용한다.
/// </summary>
[System.Serializable]
public class MonsterStat : BaseStat
{
    #region Protected_StatValue
    // 몬스터 식별 필드
    [SerializeField] protected int monsterID;
    [SerializeField] protected int monsterType;
    [SerializeField] protected string monsterName;
    // 몬스터 스탯 필드
    [SerializeField] protected int monsterLV;
    [SerializeField] protected float monsterHP;
    [SerializeField] protected float monsterATK;
    [SerializeField] protected float monsterDEF;
    [SerializeField] protected float monsterEXP;
    [SerializeField] protected float monsterSPD;
    #endregion

    #region Property_StatValue
    // 몬스터 식별 필드
    public int MonsterID { get { return monsterID; } }
    public int MonsterType { get { return monsterType; } }
    public string MonsterName { get { return monsterName; } }
    // 몬스터 스탯 필드
    public int MonsterLV { get { return monsterLV; } }
    public float MonsterHP { get { return monsterHP; } }        
    public float MonsterATK { get { return monsterATK; } }      
    public float MonsterDEF { get { return monsterDEF; } }  
    public float MonsterEXP { get { return monsterEXP; } }
    public float MonsterSPD { get { return monsterSPD; } }
    #endregion
}

public class CombatMonsterStat : MonsterStat
{
    [SerializeField] protected float attackRate;
    public float AttackRate { get { return attackRate; } }
}



//// 몬스터 현재 스탯 값
//protected float currentHP;
//protected float currentATK;
//protected float currentDEF;
//protected float currentSPD;

//// 현재 스탯 값
//public float CurrentHP { get { return currentHP; } }
//public float CurrentATK { get { return currentATK; } }
//public float CurrentDEF { get { return currentDEF; } }
//public float CurrentSPD { get { return currentSPD; } }

//public void LoseHP(int _loseValue) { }
//public void RecoveryHP(int _recoveryValue) { }