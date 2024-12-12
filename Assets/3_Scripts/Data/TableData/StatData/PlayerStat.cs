using UnityEngine;

/// <summary>
/// 파일로부터 불러와 사용하는 스탯 데이터 : 고정 데이터
/// </summary>
[System.Serializable]
public class PlayerStat : BaseStat
{
    #region Protected
    // 플레이어 고유 스탯
    [SerializeField] protected PlayerEnums.TYPEIDS typeID;
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float jumpSpeed;
    [SerializeField] protected float attackSpeed;
    protected PlayerSaveStat currentStat = null;
    #endregion

    #region Public
    // 플레이어 고유 스탯
    public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
    public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public PlayerSaveStat CurrentStat { get { return currentStat; } }
    #endregion

    #region Load Stat
    
    /// <summary>
    /// 가장 먼저 호출되어야 하는 코드 
    /// </summary>
    /// <param name="_currentStat"></param>
    public void LoadPlayerSaveStat(PlayerSaveStat _currentStat) { this.currentStat = _currentStat; }

    public void LoadPlayerStat(TableMgr tableMgr)
    {
        //TableMgr tableMgr = SharedMgr.TableMgr;
        //tableMgr.Link();
        PlayerTable.PlayerTableData tableData = new PlayerTable.PlayerTableData();
        tableData = tableMgr.character.GetPlayerTableData(typeID);

        actorName = tableData.name;
        speed = tableData.speed;
        dashSpeed = tableData.dashSpeed;
        jumpSpeed = tableData.jumpSpeed;    

        PlayerTable.PlayerStatTableData statData = new PlayerTable.PlayerStatTableData();
        statData = tableMgr.character.GetPlayerStatTableData(typeID, currentStat.currentLevel);
        attackValue = statData.attackValue;
        defenceValue = statData.defenceValue;
        criticalValue = statData.criticalValue;
        maxHp = statData.maxHp;
        level = currentStat.currentLevel;
    }
    #endregion

    /// <summary>
    /// Save Data를 Load한 이후에 호출해야 한다.
    /// </summary>

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
}

