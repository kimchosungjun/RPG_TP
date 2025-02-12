using UnityEngine;
using UtilEnums;
using PlayerEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableMgr 
{
    /*****************************/
    /******** Table Data *******/
    /*****************************/

    #region Table Data
    
    // Private 
    private PlayerTable player = new PlayerTable();
    private MonsterTable monster = new MonsterTable();
    private ItemTable item = new ItemTable();
    
    // Property
    public PlayerTable GetPlayer { get { return player; } }
    public MonsterTable GetMonster { get {  return monster; } }
    public ItemTable GetItem { get { return item; } }

    #endregion

    #region Load Data Table
    public void Init()
    {
        SharedMgr.TableMgr = this;
        ParsePlayerTable();
        ParseMonsterData();
        ParseItemData();
    }

    public void ParsePlayerTable()
    {
        GetPlayer.InitPlayerTableCsv("PlayerTable", 1, 0);
        GetPlayer.InitPlayerLevelTableCsv("PlayerLevelTable", 1, 0);
        GetPlayer.InitPlayerNormalAttackTableCsv("PlayerNormalAttackTable", 1, 0);
        GetPlayer.InitPlayerConditionSkillTableCsv("PlayerConditionSkillTable", 1, 0);
        GetPlayer.InitPlayerAttackSkillTableCsv("PlayerAttackSkillTable", 1, 0);
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

    #endregion
}
