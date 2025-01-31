using Unity.VisualScripting;
using UnityEngine;

public class ProjectileAttackAction : AttackAction
{
    [SerializeField] Transform throwPosition;
    [SerializeField] HitTriggerProjectile projectile;
    [SerializeField] bool haveParentDirection = false;

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData, HitTriggerProjectile _projectile)
    {
        this.projectile = _projectile;
        this.attackData = _attackData;
        this.conditionData = _conditionData;

        if (throwPosition == null)
            throwPosition = this.transform;

        DoAttack();
    }

    public override void DoAttack() 
    {
        Vector3 direction = Vector3.zero;
        if (haveParentDirection)
        {
            direction = throwPosition.position -this.transform.parent.position ;
            direction.y = 0;
            direction = direction.normalized;
        }
        else
        {
            direction = transform.forward;
            direction.y = 0;
            direction = direction.normalized;
        }
        projectile.SetHitData(attackData, conditionData, throwPosition.rotation, throwPosition.position, direction);
    }

    public override void StopAttack() 
    {
        projectile.gameObject.SetActive(false);
    }
}
