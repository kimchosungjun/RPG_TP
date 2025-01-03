using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateExplosion : MonoBehaviour
{
    #region Set Data
    [SerializeField] Collider thisCollider;
    [SerializeField] float moveSpeed;
    int groundLayer = (int)UtilEnums.LAYERS.GROUND;
    int enemyLayer = 0;
    TransferAttackData attackData;
    TransferConditionData conditionData;
    Vector3 explosionPosition;
    Quaternion explosionRotation;

    bool isCollide = false;
    PoolEnums.OBJECTS poolIndex;

    public void SetTransferData(TransferAttackData _attackData, TransferConditionData _conditionData,
        Vector3 _position, Quaternion _rotation, Vector3 _direction, Vector3 _explosionPosition, Quaternion _explosionRotation,float _timer,PoolEnums.OBJECTS _poolIndex,UtilEnums.LAYERS  _enemyLayer = UtilEnums.LAYERS.MONSTER)
    {
        transform.position = _position;
        transform.rotation = _rotation;

        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        thisCollider.enabled = true;
        isCollide = false;

        attackData = _attackData;   
        conditionData = _conditionData; 
        enemyLayer = (int)_enemyLayer;
        
        poolIndex = _poolIndex;
        explosionPosition = _explosionPosition;
        explosionRotation = _explosionRotation; 
        MoveTimer(_direction, _timer);
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
        if (isCollide)
            yield break;
        DoExplosion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer || other.gameObject.layer == groundLayer)
        {
            isCollide = true;
            thisCollider.enabled = false;
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
