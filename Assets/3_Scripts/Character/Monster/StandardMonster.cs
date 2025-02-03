using MonsterEnums;
using UnityEngine;

public class StandardMonster : BaseMonster
{
    [Header("Status UI : Must Link"),SerializeField] protected StandardMonsterStatusUI statusUI = null;
    protected bool isBattle = false;

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
    public override bool CanTakeDamageState()
    {
        if (IsInMonsterArea == false) return false;
        return true;
    }

    public override void ApplyStatTakeDamage(TransferAttackData _attackData) 
    {
        base.ApplyStatTakeDamage(_attackData);
        if (isBattle == false)
            isBattle = true;

    }

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

    public override void AnnounceStatusUI()
    {
        statusUI.UpdateStatusData();
    }
}
