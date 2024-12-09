using UnityEngine;

public class TestMonsterDataCreate : MonoBehaviour
{
    [SerializeField] PlayerTable.PlayerTableData[] tableData;
    [SerializeField] PlayerTable.PlayerNormalAttackData[] attackData;
    public void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "테이블 읽기"))
        {
            TableMgr mgr = new TableMgr();
            mgr.Link();
            tableData[0] = mgr.character.GetPlayerTableData(0);
            tableData[1] = mgr.character.GetPlayerTableData(1);
            tableData[2] = mgr.character.GetPlayerTableData(2);

            attackData[0] = mgr.character.GetPlayerAttackData(PlayerEnums.TYPEID.WARRIOR, 1);
        }
    }
}
