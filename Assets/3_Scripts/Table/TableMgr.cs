using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableMgr 
{
    public Table_Character character = new Table_Character();
    public void Init()
    {
#if UNITY_EDITOR
        character.Init_Csv("testCsv",1,0);
#else
        character.Init_Binary("testCsv");
#endif
    }

    public void Save()
    {
        character.Save_Binary("testCsv");
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    
}
