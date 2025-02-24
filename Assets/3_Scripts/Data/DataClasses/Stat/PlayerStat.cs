using UnityEngine;
using PlayerTableClassGroup;
using ItemEnums;
using PlayerEnums;

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
    [SerializeField] protected string atlasName;
    [SerializeField] protected string fileName;
    [SerializeField] protected WEAPONTYPE holdWeaponType;
    [SerializeField] protected BATTLE_TYPE battleType;
    [SerializeField] protected PlayerBaseActionSOData[] soDatas = new PlayerBaseActionSOData[3];
    #endregion

    #region Public
    public int HoldWeaponUniqueID { get { return currentStat.holdWeaponUniqueID; }  set { currentStat.holdWeaponUniqueID = value; } }
    public int Attack { get { return attackValue; } set { attackValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
    public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    public PlayerSaveStat GetSaveStat{ get { return currentStat; } }
    public string GetAtlasName { get { return atlasName; } }
    public string GetFileName { get { return fileName; } }

    public WEAPONTYPE GetWeaponType { get { return holdWeaponType; } }
    public BATTLE_TYPE GetBattleType { get { return battleType; } }
    public PlayerBaseActionSOData GetActionSoData(ACTION_TYPE _actionType) 
    {
        int index = (int)_actionType;
        int cnt = soDatas.Length;
        if (cnt <= index) return null;
        return soDatas[(int)_actionType]; 
    }
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

        atlasName = tableData.atlasName;
        fileName = tableData.fileName;
        holdWeaponType = tableData.GetWeaponType();
        battleType = tableData.GetBattleType();
        SharedMgr.GameCtrlMgr.GetPlayerStatCtrl?.AddPlayerStat(this);

        if(currentStat.holdWeaponUniqueID >= 0)
            SharedMgr.InventoryMgr.GetHoldWeaponData(currentStat.holdWeaponUniqueID)?.Use(_currentStat.playerTypeID);
    }

    public void SetActionSoDatas(PlayerBaseActionSOData[] _actionDatas)
    {
        if (_actionDatas == null || _actionDatas.Length == 0) return;
        soDatas = _actionDatas;
    }

    public void LevelUp()
    {
        TableMgr tableMgr = SharedMgr.TableMgr;
        PlayerTableData tableData = new PlayerTableData();
        tableData = tableMgr.GetPlayer.GetPlayerTableData(currentStat.playerTypeID);

        maxHp += tableData.increaseHP;
        if(currentStat.currentHP > 0)
            currentStat.currentHP = maxHp;
        attackValue += tableData.increaseAttack;
        defenceValue += tableData.increaseDefence;
        criticalValue += tableData.increaseCritical;
        attackSpeed += tableData.increaseAttackSpeed;
    }
    #endregion

    #region Apply WeaponStat
    WeaponIncreaseStat weaponStat = null;

    public void ApplyWeaponStat(WeaponIncreaseStat _weaponStat)
    {
        weaponStat =   _weaponStat;
        attackValue += weaponStat.GetAttackValue;
        criticalValue += weaponStat.GetCirticalValue;
    }

    public void RemoveWeaponStat()
    {
        if (weaponStat == null)
            return;

        attackValue -= weaponStat.GetAttackValue;
        criticalValue -= weaponStat.GetCirticalValue;
        weaponStat = null;
    }

    public WeaponIncreaseStat GetWeaponStat() { return weaponStat; }
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
    [SerializeField] public int holdWeaponUniqueID = -1;
    public void LevelUp()
    {
        currentLevel += 1;
        SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat(playerTypeID)?.LevelUp();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerChangeUI?.LevelUp();
    }

    public PlayerSaveStat(int _id)
    {
        playerTypeID = _id;
        currentHP = SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(_id).defaultHP;     
        currentLevel = 1;   
        currentExp = 0;       
        currentNormalAttackLevel= 1;    
        currentSkillLevel = 1;
        currentUltimateSkillLevel = 1;
    }
}

