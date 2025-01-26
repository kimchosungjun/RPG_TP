using MonsterEnums;
using UnityEngine;

public class StandardMonster : BaseMonster
{
    [Header("스탯 UI"),SerializeField] protected StandardMonsterStatusUI statusUI = null;
    
    // Set Status UI

    #region Override Life Cycle
    protected override void Awake()
    {
        base.Awake();
        statusUI.Init();
    }

    protected override void Start()
    {
        base.Start();
        statusUI.Setup(this.transform, monsterStat);
    }
    #endregion

    protected override void CreateBTStates() { }
}
