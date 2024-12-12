using MonsterEnums;
using UnityEngine;

public class NormalMonster : BaseMonster
{
    [SerializeField] protected StatusUICtrl statusUICtrl = null;
    public override void Death()
    {
        throw new System.NotImplementedException();
    }

    public override BTS DetectMovement()
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

    public override void Spawn()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }
}
