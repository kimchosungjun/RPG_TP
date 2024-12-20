using UnityEngine;

public abstract class AttackAction : MonoBehaviour
{
    [SerializeField] protected TransferAttackData[] attackDatas;
    [SerializeField] protected TransferConditionData[] conditionDatas;
    
    public virtual void SetTransferData(TransferAttackData[] _attackDatas, TransferConditionData[] _conditionDatas) 
    {
        this.attackDatas = _attackDatas;
        this.conditionDatas = _conditionDatas;  
        DoAttack();
    }

    public abstract void DoAttack();
    public abstract void StopAttack();
}
