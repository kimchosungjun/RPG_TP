using UnityEngine;

public abstract class HitBox : MonoBehaviour
{
    [Header("필수 설정 값")]
    [SerializeField, Tooltip("적 레이어")] protected UtilEnums.LAYERS enemyLayer;
    protected TransferAttackData attackData = null;
    protected TransferConditionData conditionData = null;
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData) 
    {
        this.attackData= _attackData;
        this.conditionData = _conditionData;
        Active(); 
    }
    
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData, Quaternion _lookRotate, Vector3 _position) 
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;    
        transform.position = _position;
        transform.rotation = _lookRotate;
        Active();
    }

    public abstract void Active();
    public abstract void InActive();
}
