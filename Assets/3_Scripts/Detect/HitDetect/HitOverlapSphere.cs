using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitOverlapSphere : HitBox
{
    //[SerializeField, Tooltip("Is Set Max Hit Cnt")] bool haveMaxMultipleAttack;
    //[SerializeField] int maxAttackCnt = 10;
    [SerializeField] float overlapRange = 5f;

    public void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData, 
        Vector3 _position, Quaternion _lookRotate, UtilEnums.LAYERS _enemyLayer = UtilEnums.LAYERS.MONSTER)
    {
        this.attackData = _attackData;
        this.conditionData = _conditionData;
        transform.position = _position;
        transform.rotation = _lookRotate;
        enemyLayer = _enemyLayer;
        Active();
    }

    public override void Active()
    {
        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);
        DetectEnemyColliders();
    }

    public override void InActive()
    {
        this.gameObject.SetActive(false);
    }

    public void DetectEnemyColliders()
    {
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.EXPLOSION_SFX);
        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRange, 1 << (int)enemyLayer);
        int collCnt = colliders.Length;
        if (collCnt == 0) return;
        for(int i=0; i<collCnt; i++)
        {
            BaseActor baseActor = colliders[i].GetComponentInParent<BaseActor>();
            baseActor?.TakeDamage(attackData);
        }
    }
}
