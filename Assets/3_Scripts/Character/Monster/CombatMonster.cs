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

    protected override void CreateBTStates()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Life Cycle : Start

    protected override void Start()
    {
        MonsterTable table = SharedMgr.TableMgr.GetMonster;
        MonsterTableClassGroup.MonsterInfoTableData infoTableData = table.GetMonsterInfoTableData(initMonsterData.monsterType);
        MonsterTableClassGroup.MonsterStatTableData statTableData = table.GetMonsterStatTableData(initMonsterData.monsterType);
        monsterStat.SetMonsterStat(statTableData ,initMonsterData.monsterLevel);
        monsterStatControl.MonsterStat = monsterStat;
        monsterStatControl.SetStatusUI(statusUI);
        //statusUI.Setup(this.transform, monsterStat);
        base.Start();
    }
    #endregion
}
