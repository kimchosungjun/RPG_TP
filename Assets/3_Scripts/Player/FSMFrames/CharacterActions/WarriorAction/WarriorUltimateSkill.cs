using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorUltimateSkill : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] Collider attackCollider;
    NearAttackAction nearAttackAction;

    bool isAttackState;
    float attackValue;
    //ATTACK_EFFECT_TYPES effectType;


    public void SetupData(float _attackValue, int _effectIndex)
    {
        isAttackState = false;
        UpdateData(_attackValue, _effectIndex);
        nearAttackAction = new NearAttackAction(attackCollider);
    }

    public void UpdateData(float _attackValue, int _effectIndex)
    {
        this.attackValue = _attackValue;
        //effectType = (ATTACK_EFFECT_TYPES)_effectIndex;
    }

    public void DoNormalAttack()
    {
        isAttackState = true;
        nearAttackAction.DoAttack();
    }

    public void StopNormalAttack()
    {
        isAttackState = false;
        nearAttackAction.StopAttack();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isAttackState && other.gameObject.layer == enemyLayerMask)
        {
            if (nearAttackAction.CheckCollider(other))
            {
                // To Do ~~~~~~~
                // 스탯에 맞는 공격력을 반환하여 적에게 데미지를 입힌다.
            }
        }
    }
}
