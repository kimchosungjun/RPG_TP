using UnityEngine;
using PlayerTableClasses;

/// <summary>
/// 파일로부터 불러와 사용하는 스탯 데이터 : 고정 데이터
/// </summary>
[System.Serializable]
public class PlayerStat : BaseStat
{
    #region Protected
    [SerializeField] protected int typeID;
    [SerializeField] protected float attackValue;
    [SerializeField] protected float criticalValue;
    [SerializeField] protected float attackSpeed; // 공격속도는 애니메이션 속도에 영향을 준다.
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float jumpSpeed;
    protected PlayerSaveStat currentStat = null;
    #endregion

    #region Public
    public float Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
    public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    public PlayerSaveStat GetSaveStat{ get { return currentStat; } }
    #endregion

    #region Load Stat
    
    /// <summary>
    /// 가장 먼저 호출되어야 하는 코드 
    /// </summary>
    /// <param name="_currentStat"></param>
    public void LoadPlayerSaveStat(PlayerSaveStat _currentStat) { this.currentStat = _currentStat; typeID = 0; }

    public void LoadPlayerStat()
    {
        TableMgr tableMgr = SharedMgr.TableMgr;
        PlayerTableData tableData = new PlayerTableData();
        tableData = tableMgr.Player.GetPlayerTableData(typeID);

        actorName = tableData.name;
        speed = tableData.speed;
        dashSpeed = tableData.dashSpeed;
        jumpSpeed = tableData.jumpSpeed;    

        PlayerStatTableData statData = new PlayerStatTableData();
        statData = tableMgr.Player.GetPlayerStatTableData(typeID, currentStat.currentLevel);
        attackValue = statData.attackValue;
        defenceValue = statData.defenceValue;
        criticalValue = statData.criticalValue;
        maxHp = statData.maxHp;
        attackSpeed = statData.attackSpeed;
        level = currentStat.currentLevel;
    }
    #endregion
}


/// <summary>
/// 저장하고 저장한 데이터를 불러와 사용하는 스탯 데이터 : 가변 데이터
/// </summary>
[System.Serializable]
public class PlayerSaveStat
{
    [SerializeField] public float currentHP;
    [SerializeField] public int currentLevel;
    [SerializeField] public int currentExp;
    [SerializeField] public int currentNormalAttackLevel;
    [SerializeField] public int currentSkillLevel;
    [SerializeField] public int currentUltimateSkillLevel;

    public PlayerSaveStat()
    {
        currentHP = 100;
        currentLevel = 1;
        currentExp = 0;
        currentNormalAttackLevel = 1;
        currentSkillLevel = 1;
        currentUltimateSkillLevel = 1;  
    }
}

