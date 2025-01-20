using UnityEngine;
using MonsterEnums;


public class MonsterAttack 
{
    [SerializeField] MonsterAttackActionData attackActionData = new MonsterAttackActionData();
    [SerializeField] MonsterConditionActionData conditionActionData = new MonsterConditionActionData();

    public MonsterAttackActionData GetAttackData { get { return attackActionData; } }   
    public MonsterConditionActionData GetConditionData { get { return conditionActionData; } }

    #region Set Data
    public void SetData(MonsterStat _stat, CONDITION_ACTIONS _conditionAction)
    {
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(_conditionAction), _stat.Level);
        attackActionData = null;
    }

    public void SetData(MonsterStat _stat, ATTACK_ACTIONS _attackAction)
    {
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(_attackAction), _stat.Level);
        conditionActionData = null;
    }

    public void SetData(MonsterStat _stat, ATTACK_ACTIONS _attackAction, CONDITION_ACTIONS _conditionAction)
    {
        MonsterTable monsterTable = SharedMgr.TableMgr.GetMonster;
        attackActionData.SetConditionData(monsterTable.GetMonsterAttackTableData(_attackAction), _stat.Level);
        conditionActionData.SetConditionData(monsterTable.GetMonsterConditionTableData(_conditionAction), _stat.Level);
    }
    #endregion

    #region Creator

    // Empty Creator
    public MonsterAttack() { }

    public MonsterAttack(MonsterStat _stat, ATTACK_ACTIONS _attackAction) { SetData(_stat, _attackAction); }
    public MonsterAttack(MonsterStat _stat, CONDITION_ACTIONS _conditionAction) { SetData(_stat, _conditionAction); }
    public MonsterAttack(MonsterStat _stat, ATTACK_ACTIONS _attackAction, CONDITION_ACTIONS _conditionAction) { SetData(_stat, _attackAction, _conditionAction); }

    #endregion
}
