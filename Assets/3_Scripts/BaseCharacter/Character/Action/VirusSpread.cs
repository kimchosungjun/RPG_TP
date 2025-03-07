using System.Collections;
using UnityEngine;
using MonsterEnums;

public class VirusSpread : MonsterAttackAction
{
    [SerializeField] ProjectileAttackAction[] spredAttacks;

    public override void SetData(MonsterStat _stat)
    {
        base.SetData(_stat); 
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(ATTACK_ACTIONS.VIRUS_SPREAD), stat.Level);
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(CONDITION_ACTIONS.VIRUS_SLOW), stat.Level);
    }

    public override void DoAttack()
    {
        StartCoroutine(CStartCoolDown(attackActionData.GetCoolTime));
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(attackActionData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical) * attackActionData.GetMultiplier, attackActionData.GetMaintainTime);
        TransferConditionData conditionData = new TransferConditionData();
        conditionData.SetData(stat, conditionActionData.GetEffect, conditionActionData.GetAttribute,
            conditionActionData.GetConditionType, conditionActionData.GetDefaultValue, conditionActionData.GetMaintainTime, conditionActionData.GetMultiplier);

        int projectileCnt = spredAttacks.Length;
        for (int i = 0; i < projectileCnt; i++)
        {
            HitTriggerProjectile projectile = SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.VIRUS_SPREAD).GetComponent<HitTriggerProjectile>();
            spredAttacks[i].SetTransferData(attackData, conditionData, projectile);
        }
    }
}
