using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAttackAction : AttackAction
{
    [SerializeField] Transform throwPosition;
    [SerializeField] HitBox spawnHitBox;

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData, HitThrowBox _projectile)
    {
        this.spawnHitBox = _projectile;
        this.attackData = _attackData;
        this.conditionData = _conditionData;
        DoAttack();
    }

    public override void DoAttack()
    {
        spawnHitBox.SetHitData(attackData, conditionData, throwPosition.rotation, throwPosition.position, transform.forward);
    }

    public override void StopAttack()
    {
        spawnHitBox.gameObject.SetActive(false);
    }
}
