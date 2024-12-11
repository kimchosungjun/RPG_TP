using UnityEngine;

public class TestCsvReader : MonoBehaviour
{
    [Header("플레이어 데이터 : 정해진 데이터")]
    [SerializeField] PlayerTable.PlayerTableData tableData;
    [SerializeField] PlayerTable.PlayerLevelTableData levelTable;
    [SerializeField] PlayerTable.PlayerStatTableData statData;
    [SerializeField] PlayerTable.PlayerNormalAttackTableData attackData;
    [SerializeField] PlayerTable.PlayerBuffSkillTableData skillTable;
    [SerializeField] PlayerTable.PlayerAttackSkillTableData attackSkillTable;

    [Header("플레이어 저장 데이터")]
    [SerializeField] PlayerSaveStat saveStat;

    [SerializeField] TestSO t;
    
    TableMgr tableMgr;
    private void Awake()
    {
        tableMgr = new TableMgr();
        tableMgr.Link();
        saveStat= new PlayerSaveStat(); 
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

        if (GUI.Button(new Rect(150, 0, 100, 100), "전사 저장\n 스탯 저장하기"))
        {
            tableMgr.character.SaveBinary("PlayerSaveStat", saveStat);
        }

        if (GUI.Button(new Rect(300, 0, 100, 100), "전사 저장\n스탯 불러오기"))
        {
            tableMgr.character.LoadBinary<PlayerSaveStat>("PlayerSaveStat",ref saveStat);
            t.GetStat.LoadPlayerSaveStat(saveStat);
            t.GetStat.LoadPlayerStat(tableMgr);
        }
    }
}
