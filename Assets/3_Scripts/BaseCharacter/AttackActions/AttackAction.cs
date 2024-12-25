using UnityEngine;

public abstract class AttackAction : MonoBehaviour
{
    [SerializeField] protected TransferAttackData attackData;
    [SerializeField] protected TransferConditionData conditionData;
    
    public virtual void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData) 
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;  
        DoAttack();
    }

    public abstract void DoAttack();
    public abstract void StopAttack();
}
