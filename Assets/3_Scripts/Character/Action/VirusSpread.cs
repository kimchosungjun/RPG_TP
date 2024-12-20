using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterEnums;
using MonsterTableClasses;

public class VirusSpread : MonoBehaviour
{
    CombatMonsterStat stat = null;
    [SerializeField] MonsterAttackActionData attackActionData = new MonsterAttackActionData();
    [SerializeField] MonsterConditionActionData conditionActionData = new MonsterConditionActionData();
    [SerializeField] FarThrowAttackAction spredAttack = null;
    public float coolTime = 0;
    
    // 생성시 한번만 호출 : 몬스터의 레벨을 바꿀 생각 없기 때문이다.
    public void SetData(CombatMonsterStat _stat)
    {
        if (spredAttack == null) spredAttack = GetComponentInChildren<FarThrowAttackAction>();

        // 스탯과 행동 데이터 불러오기
        this.stat= _stat;
        MonsterTable monsterTable = SharedMgr.TableMgr.Monster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(ATTACK_ACTIONS.VIRUS_SPREAD), stat.Level);
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(CONDITION_ACTIONS.VIRUS_SLOW), stat.Level);
    }

    public void Spread()
    {
        // 전달할 데이터 가공 후, 전달
        TransferAttackData[] attackData = new TransferAttackData[1];
        attackData[0] = new TransferAttackData();
        attackData[0].SetData(attackActionData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical) * attackActionData.GetMultiplier, attackActionData.GetMaintainTime);
        TransferConditionData[] conditionData = new TransferConditionData[1];
        conditionData[0] = new TransferConditionData();
        conditionData[0].SetData(stat, conditionActionData.GetEffect, conditionActionData.GetAttribute,
            conditionActionData.GetConditionType, conditionActionData.GetDefaultValue, conditionActionData.GetMaintainTime, conditionActionData.GetMultiplier);

        //throwBox.SetHitData(attackData, conditionData);
    }
}
