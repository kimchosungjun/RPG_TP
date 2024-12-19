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
    [SerializeField] protected CombatMonsterStat monsterStat = null;
    public CombatMonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }

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
        //monsterLevel = infoTableData.monsterLevels[monsterLevelIndex];
        MonsterTableClasses.CombatMonsterStatTableData statTableData = table.GetCombatMonsterStatTableData(monsterType, monsterLevel);
        monsterStat.SetMonsterStat(statTableData);
        monsterStatControl.MonsterStat = monsterStat;
        monsterStatControl.SetStatusUI(statusUI);
        statusUI.Setup(this.transform, monsterStat);
        base.Start();
    }
    #endregion
}
