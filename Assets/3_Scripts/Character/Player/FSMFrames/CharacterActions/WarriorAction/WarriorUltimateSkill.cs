using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorUltimateSkill : NearAttackShockwaveAction
{
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] Collider attackCollider;
    PlayerAttackSkillActionSOData soData;
    PlayerStat playerStat;
    TransferAttackData attackData = new TransferAttackData();

    public void SetStat(PlayerStat _playerStat, PlayerAttackSkillActionSOData _soData)
    {
        playerStat = _playerStat;
        soData = _soData;
    }

    //public override void DoAction()
    //{
    //    float damageValue = soData.GetActionMultiplier * playerStat.Attack;
    //    attackData.SetData(soData.GetAttackEffectType,damageValue,soData.GetMaintainEffectTime);
    //    base.DoAction();
    //}

    public override void StopAttack()
    {
        base.StopAttack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayerMask)
        {
            //if (CheckCollider(other))
            //{
            //    // To Do ~~~~~~~
            //    // 스탯에 맞는 공격력을 반환하여 적에게 데미지를 입힌다.
            //}
        }
    }
}
