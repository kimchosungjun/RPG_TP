using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitChaseOverlap : HitBox
{
    bool isExplosion = false;
    [SerializeField] float overlapRange = 3f;
    [SerializeField] ParticleAction particleAction;
    public void SetHitData(TransferAttackData _attackData, TransferConditionData _conditionData,
        Vector3 _position, Quaternion _lookRotate, UtilEnums.LAYERS _enemyLayer = UtilEnums.LAYERS.MONSTER)
    {
        particleAction.DoParticle(11f);
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
        isExplosion = false;
        particleAction.DoParticle();
        DoExplosion(6.3f);
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.DRAGON_GUIDED_SFX);
    }

    public override void InActive()
    {
        this.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (isExplosion == false)
        {
            this.transform.position = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        }   
    }

    public void DoExplosion(float _time) { Invoke("DetectEnemyColliders", _time); }

    public void DetectEnemyColliders()
    {
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.EXPLOSION_SFX);
        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRange, 1 << (int)enemyLayer);
        int collCnt = colliders.Length;
        if (collCnt == 0) return;
        for (int i = 0; i < collCnt; i++)
        {
            BaseActor baseActor = colliders[i].GetComponentInParent<BaseActor>();
            baseActor?.TakeDamage(attackData);
        }

        isExplosion = true;
        attackData = null;
        conditionData = null;
    }
}
