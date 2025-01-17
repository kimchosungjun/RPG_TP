using UnityEngine;

public class TestCsvReader : MonoBehaviour
{
    TableMgr tableMgr;
    [SerializeField] PlayerNormalAttackActionSOData attackActionSO;
    [SerializeField] PlayerAttackSkillActionSOData skillActionSO;
    [SerializeField] PlayerConditionActionSOData buffActionSO;
    private void Awake()
    {
        tableMgr = new TableMgr();
        tableMgr.LinkPlayerTable();
    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "전사 테이블 읽기"))
        {
            //skillActionSO.SetSOData(tableMgr.GetPlayer.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE, 1));
            //attackActionSO.SetSOData(tableMgr.GetPlayer.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.WARRIOR, 2));
            //buffActionSO.SetSOData(tableMgr.GetPlayer.GetPlayerBuffSkillTableData(PlayerEnums.CONDITION_SKILLS.WARRIOR_ROAR, 3));
        }

        if (GUI.Button(new Rect(150, 0, 100, 100), "전사 저장\n 스탯 저장하기"))
        {
           
        }

        if (GUI.Button(new Rect(300, 0, 100, 100), "전사 저장\n스탯 불러오기"))
        {

        }
    }
}
