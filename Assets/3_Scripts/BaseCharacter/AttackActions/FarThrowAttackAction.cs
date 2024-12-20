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
            projectiles[i].SetHitData(attackDatas[0], conditionDatas[0], throwPositions[i].rotation, throwPositions[i].position, transform.forward);
        }
    }

    public override void StopAttack() 
    {
        int projectileCnt = throwPositions.Length;
        for (int i = 0; i < projectileCnt; i++)
        {
            projectiles[i].gameObject.SetActive(false);
        }
    }
}
