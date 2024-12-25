using UnityEngine;

public class FarThrowAttackAction : AttackAction
{
    [SerializeField] Transform throwPosition;
    [SerializeField] HitThrowBox projectile;

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData, HitThrowBox _projectile)
    {
        this.projectile = _projectile;
        this.attackData = _attackData;
        this.conditionData = _conditionData;
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
