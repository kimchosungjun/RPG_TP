using UnityEngine;

public abstract class HitBox : MonoBehaviour
{
    [Header("필수 설정 값")]
    [SerializeField, Tooltip("적 레이어")] protected UtilEnums.LAYERS enemyLayer;
    protected TransferAttackData attackData = null;
    protected TransferConditionData conditionData = null;
    protected Vector3 moveDirection;
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData, UtilEnums.LAYERS _enemyLayer = UtilEnums.LAYERS.MONSTER) 
    {
        this.attackData= _attackData;
        this.conditionData = _conditionData;
        enemyLayer = _enemyLayer;
        Active(); 
    }
    
    public virtual void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData, Quaternion _lookRotate, Vector3 _position , Vector3 _direction, UtilEnums.LAYERS _enemyLayer = UtilEnums.LAYERS.MONSTER) 
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;    
        transform.position = _position;
        transform.rotation = _lookRotate;
        moveDirection = _direction;
        enemyLayer = _enemyLayer;
        Active();
    }

    public abstract void Active();
    public abstract void InActive();
}
