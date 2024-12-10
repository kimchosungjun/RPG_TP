using UnityEngine;
using UtilEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableMgr 
{
    public PlayerTable character = new PlayerTable();
    
    public void Init() 
    {
        SharedMgr.TableMgr = this;
        // To Do ~~ Load Player Game Data
    }

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

    #region Test Function
    /// <summary>
    /// Row : 행, Col : 열
    /// </summary>
    public void Link()
    {
#if UNITY_EDITOR
        character.InitPlayerTableCsv("PlayerTable", 1, 0);
        character.InitPlayerStatTableCsv("WarriorStatTable", 1, 0, PlayerEnums.TYPEID.WARRIOR);
#else
        character.Init_Binary("testCsv");
#endif
    }

    public void LoadPlayerData(/*PlayerEnums.TYPEID _typeID*/)
    {
        character.InitPlayerTableCsv("PlayerTableCsv", 1, 0);
        
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
