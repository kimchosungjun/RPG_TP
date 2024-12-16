using MonsterEnums;
using UnityEngine;

public class NormalMonster : BaseMonster
{
    [SerializeField] protected NormalMonsterStatusUI statusUI = null;

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

    public override void Spawn(Vector3 _spawnPosition)
    {
        throw new System.NotImplementedException();
    }

    protected override void CreateBTStates()
    {
        throw new System.NotImplementedException();
    }
}
