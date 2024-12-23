using UnityEngine;

/// <summary>
/// 스탯을 정하는 클래스
/// </summary>
public class CombatMonster : StandardMonster
{
    /******************************************/
    /****** 비전투 유닛 공통 변수  ********/
    /******************************************/

    #region Value : Stat

        
    [Header("거리")]
    [SerializeField] protected float detectRange;
    #endregion

    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Life Cycle : Start

    protected override void Start()
    {
        MonsterTable table = SharedMgr.TableMgr.Monster;
        MonsterTableClasses.MonsterInfoTableData infoTableData = table.GetMonsterInfoTableData(monsterType);
        MonsterTableClasses.MonsterStatTableData statTableData = table.GetMonsterStatTableData(monsterType);
        monsterStat.SetMonsterStat(statTableData ,monsterLevel);
        monsterStatControl.MonsterStat = monsterStat;
        monsterStatControl.SetStatusUI(statusUI);
        statusUI.Setup(this.transform, monsterStat);
        base.Start();
    }
    #endregion
}
