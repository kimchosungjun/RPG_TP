using UnityEngine;

public abstract class HitBox : MonoBehaviour
{
    [Header("필수 설정 값")]
    [SerializeField, Tooltip("적 레이어")] protected UtilEnums.LAYERS enemyLayer;
    protected TransferAttackData attackData = null;

    public virtual void SetHitData(TransferAttackData _attackData) { attackData = _attackData; Active(); }   

    public virtual void SetHitData(TransferAttackData _attackData, Quaternion _lookRotate, Vector3 _position) 
    {
        attackData = _attackData;
        transform.position = _position;
        transform.rotation = _lookRotate;
        Active();
    }

    public abstract void Active();
    public abstract void InActive();
}
