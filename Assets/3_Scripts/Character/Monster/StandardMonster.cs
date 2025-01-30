using MonsterEnums;
using UnityEngine;

public class StandardMonster : BaseMonster
{
    [Header("Status UI : Must Link"),SerializeField] protected StandardMonsterStatusUI statusUI = null;

    #region Override Life Cycle
    protected override void Awake()
    {
        base.Awake();
        if(statusUI==null)
            statusUI = GetComponentInChildren<StandardMonsterStatusUI>();   
        statusUI.Init();
    }

    protected override void Start()
    {
        base.Start();
        statusUI.Setup(this.transform, monsterStat);
    }
    #endregion

    #region Battle Field
    public override void AnnounceInMonsterArea()
    {
        base.AnnounceInMonsterArea();
        statusUI.DecideActiveState(true);
    }

    public override void AnnounceOutMonsterArea()
    {
        base.AnnounceOutMonsterArea();
        statusUI.DecideActiveState(false);
        ReturnToSpawnPosition();
    }
    #endregion

    protected override void CreateStates() { }
}
