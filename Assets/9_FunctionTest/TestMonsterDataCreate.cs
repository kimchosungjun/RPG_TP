using UnityEngine;

public class TestMonsterDataCreate : MonoBehaviour
{
    [SerializeField] PlayerTable.PlayerTableData tableData;
    [SerializeField] PlayerTable.PlayerLevelTableData levelTable;
    [SerializeField] PlayerTable.PlayerStatTableData statData;
    [SerializeField] PlayerTable.PlayerNormalAttackTableData attackData;
    [SerializeField] PlayerTable.PlayerBuffSkillTableData skillTable;
    [SerializeField] PlayerTable.PlayerAttackSkillTableData attackSkillTable;

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
            levelTable = tableMgr.character.GetPlayerLevelTableData();
            statData = tableMgr.character.GetPlayerStatTableData(PlayerEnums.TYPEID.WARRIOR, 10);
            attackData = tableMgr.character.GetPlayerNormalAttackData(PlayerEnums.TYPEID.WARRIOR, 3);
            skillTable = tableMgr.character.GetPlayerBuffSkillTableData(PlayerEnums.BUFF_SKILL.WARRIOR_ROAR, 3);
            attackSkillTable = tableMgr.character.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILL.WARRIOR_ULTIMATE, 2);
        }

        //if (GUI.Button(new Rect(150, 0, 100, 100), "전사 스탯 "))
        //{
        //    statData = tableMgr.character.GetPlayerStatTableData(PlayerEnums.TYPEID.WARRIOR, 1);
        //}
    }
}
