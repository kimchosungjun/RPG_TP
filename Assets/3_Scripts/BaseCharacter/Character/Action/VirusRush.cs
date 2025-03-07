using UnityEngine;
using MonsterEnums;

public class VirusRush : MonsterAttackAction
{
    [SerializeField] TriggerAttackAction nearAttack = null;

    public override void SetData(MonsterStat _stat)
    {
        base.SetData(_stat);
        if (nearAttack == null) nearAttack = GetComponentInChildren<TriggerAttackAction>();
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(ATTACK_ACTIONS.VIRUS_RUSH), stat.Level);
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(CONDITION_ACTIONS.VIRUS_SLOW), stat.Level);
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
        SetAttackParticle();
        nearAttack.DoAttack();
    }

    public override void StopAttack() { nearAttack.StopAttack(); }

    public override void SetAttackParticle()
    {
        Vector3 position = transform.position + transform.forward + Vector3.up;
        Transform particleTF = SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.MON_BITE);
        particleTF.position = position;
        particleTF.gameObject.SetActive(true);
    }
}
