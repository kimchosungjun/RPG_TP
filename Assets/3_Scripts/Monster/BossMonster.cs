using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : BaseMonster
{
    [SerializeField] protected MonsterStatusUICtrl statusUICtrl = null;

    public void Test()
    {
        
    }
    #region Override
    public override void Death()
    {
        throw new System.NotImplementedException();
    }

    public override E_BTS DetectMovement()
    {
        throw new System.NotImplementedException();
    }

    public override E_BTS DetectPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override E_BTS IdleMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery()
    {
        throw new System.NotImplementedException();
    }

    public override void Spawn()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
