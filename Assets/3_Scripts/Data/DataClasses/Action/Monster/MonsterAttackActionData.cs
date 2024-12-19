using UnityEngine;
using MonsterTableClasses;

[System.Serializable]
public class MonsterAttackActionData : MonsterBaseActionData
{
    [SerializeField] protected int actionID;
    [SerializeField] protected float defaultValueIncrease;

    public MonsterAttackActionData(MonsterAttackTableData _tableData)
    {
        actionID = _tableData.ID;
        attribute = _tableData.attribute;
        multiplier = _tableData.multiplier;
        effect = _tableData.effect;
        maintainTime = _tableData.maintainTime;
        coolTime = _tableData.coolTime;
        defaultValue = _tableData.defaultDamage;
        defaultValueIncrease = _tableData.damageIncrease;
    }
    public void SetConditionData(MonsterAttackTableData _tableData)
    {
        actionID = _tableData.ID;
        attribute = _tableData.attribute;
        multiplier = _tableData.multiplier;
        effect = _tableData.effect;
        maintainTime = _tableData.maintainTime;
        coolTime = _tableData.coolTime;
        defaultValue = _tableData.defaultDamage;
        defaultValueIncrease = _tableData.damageIncrease;
    }
}
