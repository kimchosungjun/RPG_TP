using UnityEngine;
using UtilEnums;
using PlayerEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableMgr 
{
    #region Private Table
    private PlayerTable player = new PlayerTable();
    
    private MonsterTable monster = new MonsterTable();
    #endregion

    #region Property
    public PlayerTable Player { get { return player; } }
    public MonsterTable Monster { get {  return monster; } }
    #endregion

    public void Init() 
    {
        SharedMgr.TableMgr = this;
        LinkPlayerTable();
        ParseMonsterData();
    }

    private void ParseMonsterData()
    {
        Monster.InitMonsterDropTableCsv("MonsterDropTable", 1, 0);
        Monster.InitMonsterInfoTableCsv("MonsterInfoTable", 1, 0);
        Monster.InitNonCombatMonsterStatTableCsv("NonCombatMonsterStatTable", 1, 0);
        Monster.InitCombatMonsterStatTableCsv("CombatMonsterStatTable", 1, 0);
        Monster.InitMonsterAttackTableCsv("MonsterAttackTable", 1, 0);
        Monster.InitMonsterConditionTableCsv("MonsterConditionTable", 1, 0);
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
        Player.InitPlayerTableCsv("PlayerTable", 1, 0);
        Player.InitPlayerLevelTableCsv("PlayerLevelTable", 1, 0);
        LinkWarriorTable();
#else
#endif
    }

    public void LinkWarriorTable()
    {
        Player.InitPlayerStatTableCsv("WarriorStatTable", 1, 0, TYPEIDS.WARRIOR);
        Player.InitPlayerNormalAttackTableCsv("WarriorNormalAttackTable", 1, 0, TYPEIDS.WARRIOR);
        Player.InitPlayerBuffSkillTableCsv("WarriorBuffSkillTable", 1, 0, BUFF_SKILLS.WARRIOR_ROAR, TYPEIDS.WARRIOR);
        Player.InitPlayerAttackSkillTableCsv("WarriorAttackUltimateSkillTable", 1, 0, ATTACK_SKILLS.WARRIOR_ULTIMATE);
    }

    public void LoadPlayerData(/*PlayerEnums.TYPEID _typeID*/)
    {
        Player.InitPlayerTableCsv("PlayerTableCsv", 1, 0);
        
        //.InitAttackCsv("WarriorAttackTableCsv", 1, 0, PlayerEnums.TYPEID.WARRIOR);
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
