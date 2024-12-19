using UnityEngine;

public class FarThrowAttackAction : AttackAction
{
    [SerializeField] Transform[] throwPositions;
    [SerializeField] HitThrowBox[] projectiles;
    public override void DoAttack()
    {
        int projectileCnt = throwPositions.Length;
        for(int i = 0; i < projectileCnt; i++) 
        {
           // projectiles[i].SetHitData();
        }
    }

    public override void StopAttack()
    {
        throw new System.NotImplementedException();
    }
}
