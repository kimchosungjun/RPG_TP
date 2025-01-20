using UnityEngine;

public abstract class AttackAction : MonoBehaviour
{
    protected TransferAttackData attackData;
    protected TransferConditionData conditionData;
    
    public virtual void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData, bool _immediateAttack = true) 
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;  
        if(_immediateAttack)
            DoAttack();
    }

    public abstract void DoAttack();
    public abstract void StopAttack();
}
