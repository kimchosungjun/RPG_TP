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
    public override NODESTATES IdleMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        throw new System.NotImplementedException();
    }

    protected override void CreateBTStates()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
