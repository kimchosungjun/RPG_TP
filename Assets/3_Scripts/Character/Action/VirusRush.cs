using UnityEngine;
using MonsterEnums;
using System.Collections;
public class VirusRush : MonoBehaviour
{
    MonsterStat stat;
    [SerializeField] MonsterAttackActionData attackActionData = new MonsterAttackActionData();
    [SerializeField] MonsterConditionActionData conditionActionData = new MonsterConditionActionData();
    [SerializeField] NearTriggerAttackAction nearAttack = null;
    public float coolTime = 0; 
    [SerializeField] protected bool isCoolDown = true;
    public bool GetCoolDown { get { return isCoolDown; } }

    // 생성시 한번만 호출 : 몬스터의 레벨을 바꿀 생각 없기 때문이다.
    public void SetData(MonsterStat _stat)
    {
        if (nearAttack == null) nearAttack = GetComponentInChildren<NearTriggerAttackAction>();

        // 스탯과 행동 데이터 불러오기
        this.stat = _stat;
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(ATTACK_ACTIONS.VIRUS_RUSH), stat.Level);
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(CONDITION_ACTIONS.VIRUS_SLOW), stat.Level);
    }

    public void StartRush()
    {
        StartCoroutine(CStartCoolDown(2f));
        // 전달할 데이터 가공 후, 전달
        TransferAttackData attackData = new TransferAttackData();
        attackData = new TransferAttackData();
        attackData.SetData(attackActionData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical) * attackActionData.GetMultiplier, attackActionData.GetMaintainTime);
        TransferConditionData conditionData = new TransferConditionData();
        conditionData.SetData(stat, conditionActionData.GetEffect, conditionActionData.GetAttribute,
            conditionActionData.GetConditionType, conditionActionData.GetDefaultValue, conditionActionData.GetMaintainTime, conditionActionData.GetMultiplier);
        nearAttack.SetTransferData(attackData, conditionData);
    }

    public void StopRush() { nearAttack.StopAttack(); }
    
    IEnumerator CStartCoolDown(float _coolTime)
    {
        isCoolDown = false;
        yield return new WaitForSeconds(_coolTime);
        isCoolDown = true;
    }

    private void OnDisable()
    {
        isCoolDown = true;
    }
}
