using UnityEngine;
using PlayerTableClassGroup;
using MonsterEnums;

/// <summary>
/// 파일로부터 불러와 사용하는 스탯 데이터 : 고정 데이터
/// </summary>
[System.Serializable]
public class PlayerStat : BaseStat
{
    #region Protected
    [SerializeField] protected int attackValue;
    [SerializeField] protected float criticalValue;
    [SerializeField] protected float attackSpeed; // 공격속도는 애니메이션 속도에 영향을 준다.
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float jumpSpeed;
    [SerializeField] protected PlayerSaveStat currentStat = null;
    #endregion

    #region Public
    public int Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
    public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    public PlayerSaveStat GetSaveStat{ get { return currentStat; } }
    #endregion

    #region Load Stat
    
    public void LoadPlayerStat(PlayerSaveStat _currentStat)
    {
        int level = _currentStat.currentLevel - 1; // 시작레벨 1을 뺀다.
        this.currentStat = _currentStat;

        TableMgr tableMgr = SharedMgr.TableMgr;
        PlayerTableData tableData = new PlayerTableData();
        tableData = tableMgr.GetPlayer.GetPlayerTableData(currentStat.playerTypeID);

        actorName = tableData.name;
        speed = tableData.speed;
        dashSpeed = tableData.dashSpeed;
        jumpSpeed = tableData.jumpSpeed;

        maxHp = tableData.defaultHP + level * tableData.increaseHP;
        attackValue = tableData.defaultAttack + level * tableData.increaseAttack;
        defenceValue = tableData.defaultDefence + level * tableData.increaseDefence;
        criticalValue = tableData.defaultCritical + level * tableData.increaseCritical;
        attackSpeed = tableData.defaultAttackSpeed+ level * tableData.increaseAttackSpeed;
    }
    #endregion
}


/// <summary>
/// 저장하고 저장한 데이터를 불러와 사용하는 스탯 데이터 : 가변 데이터
/// </summary>
[System.Serializable]
public class PlayerSaveStat
{
    [SerializeField] public int playerTypeID;
    [SerializeField] public int currentHP;
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
        playerTypeID = 0;
    }

    public PlayerSaveStat(int _id, int _curHP)
    {
        playerTypeID = _id;
        currentHP = _curHP; 
        currentLevel = 1;   
        currentExp = 0;       
        currentNormalAttackLevel= 1;    
        currentSkillLevel = 1;
        currentUltimateSkillLevel = 1;
    }
}

