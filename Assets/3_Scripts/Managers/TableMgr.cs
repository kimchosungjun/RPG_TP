using UnityEngine;
using UtilEnums;
using PlayerEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableMgr 
{
    #region Private Table
    private AccountSaveData accountSaveData;
    private PlayerTable player = new PlayerTable();
    private MonsterTable monster = new MonsterTable();
    private ItemTable item = new ItemTable();
    #endregion

    #region Property
    public PlayerTable GetPlayer { get { return player; } }
    public MonsterTable GetMonster { get {  return monster; } }
    public ItemTable GetItem { get { return item; } }
    public AccountSaveData AccountSaveData { get { return accountSaveData; } set { accountSaveData = value; } }
    #endregion

    public void Init() 
    {
        SharedMgr.TableMgr = this;
        LinkPlayerTable();
        ParseMonsterData();
        ParseItemData();
    }

    private void ParseMonsterData()
    {
        monster.InitMonsterDropTableCsv("MonsterDropTable", 1, 0);
        monster.InitMonsterInfoTableCsv("MonsterInfoTable", 1, 0);
        monster.InitMonsterStatTableCsv("MonsterStatTable", 1, 0);
        monster.InitMonsterAttackTableCsv("MonsterAttackTable", 1, 0);
        monster.InitMonsterConditionTableCsv("MonsterConditionTable", 1, 0);
    }

    private void ParseItemData()
    {
        item.InitEtcTableCsv("EtcTable", 1, 0);
        item.InitConsumeTableCsv("ComsumeTable", 1, 0);
        item.InitWeaponTableCsv("WeaponTable", 1, 0);
        item.InitWeaponUpgradeTableCsv("WeaponUpgradeTable", 1, 0);
    }

    #region Test Function

    public void LoadTableData(TABLE_FOLDER_TYPES _tableType)
    {
        switch (_tableType)
        {
            case TABLE_FOLDER_TYPES.PLAYER:
                LoadPlayerData();
                break;
            case TABLE_FOLDER_TYPES.MONSTER:
                break;
        }
    }

    /// <summary>
    /// Row : 행, Col : 열
    /// </summary>
    public void LinkPlayerTable()
    {
#if UNITY_EDITOR
        GetPlayer.InitPlayerTableCsv("PlayerTable", 1, 0);
        GetPlayer.InitPlayerLevelTableCsv("PlayerLevelTable", 1, 0);
        GetPlayer.InitPlayerNormalAttackTableCsv("PlayerNormalAttackTable", 1, 0);
        GetPlayer.InitPlayerConditionSkillTableCsv("PlayerConditionSkillTable", 1, 0);
        GetPlayer.InitPlayerAttackSkillTableCsv("PlayerAttackSkillTable", 1,0);
#else
#endif
    }

    public void LoadPlayerData(/*PlayerEnums.TYPEID _typeID*/)
    {
        GetPlayer.InitPlayerTableCsv("PlayerTableCsv", 1, 0);
    }

    public void Save()
    {
        //character.SaveBinary<PlayerTable>("PlayerTableCsv");
#if UNITY_EDITOR
        //AssetDatabase.Refresh();
#endif
    }
    #endregion
}
