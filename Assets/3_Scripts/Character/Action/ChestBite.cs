using UnityEngine;
using MonsterEnums;

public class ChestBite : MonsterAttackAction
{
    [SerializeField] TriggerAttackAction nearAttack = null;

    public override void SetData(MonsterStat _stat)
    {
        base.SetData(_stat);
        if (nearAttack == null) nearAttack = GetComponentInChildren<TriggerAttackAction>();
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(ATTACK_ACTIONS.CHEST_BITE), stat.Level);
    }

    public override void DoAttack()
    {
        StartCoroutine(CStartCoolDown(attackActionData.GetCoolTime));
        TransferAttackData attackData = new TransferAttackData();
        attackData = new TransferAttackData();
        attackData.SetData(attackActionData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical) * attackActionData.GetMultiplier, attackActionData.GetMaintainTime);
        TransferConditionData conditionData = new TransferConditionData();
        conditionData.SetData(stat, conditionActionData.GetEffect, conditionActionData.GetAttribute,
            conditionActionData.GetConditionType, conditionActionData.GetDefaultValue, conditionActionData.GetMaintainTime, conditionActionData.GetMultiplier);
        nearAttack.SetTransferData(attackData, conditionData);
        nearAttack.DoAttack();
    }

    public override void StopAttack() { nearAttack.StopAttack(); }
}
