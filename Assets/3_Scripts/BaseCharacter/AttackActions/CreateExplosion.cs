using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateExplosion : MonoBehaviour
{
    #region Set Data
    [SerializeField] float moveSpeed;
    int groundLayer = (int)UtilEnums.LAYERS.GROUND;
    int enemyLayer = 0;
    TransferAttackData attackData;
    TransferConditionData conditionData;
    Vector3 explosionPosition;
    Quaternion explosionRotation;

    PoolEnums.OBJECTS poolIndex;

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData,
        Vector3 _position, Quaternion _rotation, Vector3 _direction, Vector3 _explosionPosition, Quaternion _explosionRotation,float _timer,PoolEnums.OBJECTS _poolIndex,UtilEnums.LAYERS  _enemyLayer = UtilEnums.LAYERS.MONSTER)
    {
        transform.position = _position;
        transform.rotation = _rotation;

        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        attackData = _attackData;   
        conditionData = _conditionData; 
        enemyLayer = (int)_enemyLayer;
        
        poolIndex = _poolIndex;
        explosionPosition = _explosionPosition;
        explosionRotation = _explosionRotation; 
        MoveTimer(_direction, _timer);
    }

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData,
    Vector3 _position, Quaternion _rotation, Vector3 _explosionPosition, Quaternion _explosionRotation, float _timer, PoolEnums.OBJECTS _poolIndex, UtilEnums.LAYERS _enemyLayer = UtilEnums.LAYERS.MONSTER)
    {
        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        attackData = _attackData;
        conditionData = _conditionData;
        enemyLayer = (int)_enemyLayer;
        transform.position = _position;
        transform.rotation = _rotation;
        poolIndex = _poolIndex;
        NotMoveTimer(_timer);
    }
    #endregion

    #region Explosion

    public void MoveTimer(Vector3 _direction, float _timer) { StartCoroutine(CMoveTimer(_direction, _timer)); }

    IEnumerator CMoveTimer(Vector3 _direction, float _timer)
    {
        float time = 0f;
        while (time <=  _timer)
        {
            time += Time.deltaTime;
            transform.position += _direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        DoExplosion();
    }

    public void NotMoveTimer(float _timer)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Debug.Log(other.gameObject.layer);
            Debug.Log(other.gameObject.name);
        }

        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == groundLayer)
        {           

            DoExplosion();
        }
    }

    public void DoExplosion()
    {
        Transform poolTrf = SharedMgr.PoolMgr.GetPool(poolIndex);
        poolTrf.GetComponent<HitOverlapSphere>().SetHitData
            (attackData, conditionData, explosionPosition, explosionRotation);
        poolTrf.GetComponent<ParticleAction>().SetParticleTime();
        this.gameObject.SetActive(false);
    }
    #endregion
}
