using MonsterEnums;
using UnityEngine;

public class EliteMonster : BaseMonster
{
    [SerializeField] protected StatusUI statusUICtrl = null;

    #region Override
    public override void Death()
    {
        throw new System.NotImplementedException();
    }

    public override BTS DetectPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override BTS IdleMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery()
    {
        throw new System.NotImplementedException();
    }

    protected override void CreateBTStates()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
