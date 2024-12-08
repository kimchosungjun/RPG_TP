using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TableReload : MonoBehaviour
{
    // 메뉴로 만들어야하므로 무조건 static선언
    [MenuItem("Cs_Util/Table/Csv & F1",false,1)]
    static public void ParseTableCsv()
    {
        TableMgr tableMgr = new TableMgr();
        tableMgr.Init();
        tableMgr.Save();
    }
}
