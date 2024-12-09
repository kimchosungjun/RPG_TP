using UnityEngine;
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

    #region Test Function
    public void Link()
    {
#if UNITY_EDITOR
        character.InitCsv("PlayerTableCsv", 1, 0);
        character.InitAttackCsv("WarriorAttackTableCsv", 1, 0, PlayerEnums.TYPEID.WARRIOR);
#else
        character.Init_Binary("testCsv");
#endif
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
