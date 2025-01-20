using UnityEngine;

public abstract class HitBox : MonoBehaviour
{
    [Header("필수 설정 값")]
    [SerializeField, Tooltip("적 레이어")] protected UtilEnums.LAYERS enemyLayer;
    [SerializeField] protected TransferAttackData attackData = null;
    [SerializeField] protected TransferConditionData conditionData = null;
    protected Vector3 moveDirection;
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData) 
    {
        this.attackData= _attackData;
        this.conditionData = _conditionData;
        Active(); 
    }
    
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData, Quaternion _lookRotate, Vector3 _position , Vector3 _direction) 
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;    
        transform.position = _position;
        transform.rotation = _lookRotate;
        moveDirection = _direction;
        Active();
    }

    public void ResetHitData()
    {
        attackData = null;
        conditionData = null;
    }

    public abstract void Active();
    public abstract void InActive();
}
