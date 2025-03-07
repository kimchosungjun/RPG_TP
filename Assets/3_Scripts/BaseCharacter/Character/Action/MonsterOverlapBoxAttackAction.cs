using UnityEngine;
using MonsterEnums;
using PoolEnums;

public class MonsterOverlapBoxAttackAction : MonsterAttackAction
{
    [Header("Overlap Size")]
    [SerializeField] Vector3 offSet;
    [SerializeField] Vector3 boxSize;
    [SerializeField] ATTACK_ACTIONS attackAction = ATTACK_ACTIONS.NONE;
    [SerializeField] CONDITION_ACTIONS conditionAction = CONDITION_ACTIONS.NONE;
    [SerializeField] OBJECTS particleType = OBJECTS.NONE;

    public override void SetData(MonsterStat _stat)
    {
        base.SetData(_stat);
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;

        if(attackAction != ATTACK_ACTIONS.NONE)
            attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(attackAction), stat.Level);

        if(conditionAction != CONDITION_ACTIONS.NONE)
            conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(conditionAction), stat.Level);
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
        Collider[] colls = Physics.OverlapBox(transform.position + offSet, boxSize / 2, Quaternion.identity, 1<<(int)UtilEnums.LAYERS.PLAYER);
        SetAttackParticle();
        if (colls.Length == 0)
            return;
        colls[0].GetComponent<BaseActor>()?.TakeDamage(attackData);
    }

    public override void StopAttack() { }

    public override void SetAttackParticle()
    {
        Vector3 position = transform.position + transform.forward + Vector3.up;
        Transform particleTF = SharedMgr.PoolMgr.GetPool(particleType);
        particleTF.position = position;
        particleTF.gameObject.SetActive(true);
    }
}
