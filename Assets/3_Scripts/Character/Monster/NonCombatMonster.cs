using UnityEngine;

/// <summary>
/// 스탯을 정하는 클래스
/// </summary>
public class NonCombatMonster : StandardMonster
{
    /******************************************/
    /****** 비전투 유닛 공통 변수  ********/
    /******************************************/

    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Life Cycle : Start

    protected override void Start()
    {
        MonsterTable table = SharedMgr.TableMgr.GetMonster;
        MonsterTableClassGroup.MonsterInfoTableData infoTableData = table.GetMonsterInfoTableData(monsterType);
        MonsterTableClassGroup.MonsterStatTableData statTableData = table.GetMonsterStatTableData(monsterType);
        monsterStat.SetMonsterStat(statTableData, monsterLevel);
        monsterStatControl.MonsterStat = monsterStat;
        monsterStatControl.SetStatusUI(statusUI);
        statusUI.Setup(this.transform, monsterStat);
        base.Start();
    }
    #endregion
}
