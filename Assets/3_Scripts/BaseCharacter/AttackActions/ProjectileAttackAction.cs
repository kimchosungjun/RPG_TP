using UnityEngine;

public class ProjectileAttackAction : AttackAction
{
    [SerializeField] Transform throwPosition;
    [SerializeField] HitTriggerProjectile projectile;

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
        projectile.SetHitData( attackData,  conditionData,  throwPosition.rotation, throwPosition.position, transform.forward);
    }

    public override void StopAttack() 
    {
        projectile.gameObject.SetActive(false);
    }
}
