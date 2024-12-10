using UnityEngine;

public class TestMonsterDataCreate : MonoBehaviour
{
    [SerializeField] PlayerTable.PlayerTableData tableData;
    [SerializeField] PlayerTable.PlayerStatTableData statData;

    TableMgr tableMgr;
    private void Awake()
    {
        tableMgr = new TableMgr();
        tableMgr.Link();
    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "전사 테이블 읽기"))
        {
            tableData = tableMgr.character.GetPlayerTableData(PlayerEnums.TYPEID.WARRIOR);
        }

        if (GUI.Button(new Rect(150, 0, 100, 100), "전사 스탯 읽기"))
        {
            statData = tableMgr.character.GetPlayerStatTableData(PlayerEnums.TYPEID.WARRIOR, 1);
        }
    }
}
