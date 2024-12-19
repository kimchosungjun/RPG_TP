using MonsterTableClasses;
using UnityEngine;

public class MonsterConditionActionData : MonsterBaseActionData
{
    [SerializeField] protected int actionID;
    [SerializeField] protected int conditionType;
    [SerializeField] protected float defaultValueIncrease;

    public MonsterConditionActionData() { }
    public void SetConditionData(MonsterConditionTableData _tableData)
    {
        actionID = _tableData.ID;
        attribute = _tableData.attribute;
        multiplier = _tableData.multiplier;
        effect = _tableData.effect;
        maintainTime = _tableData.maintainTime; 
        coolTime = _tableData.coolTime;
        defaultValue = _tableData.defaultConditionValue;
        conditionType = _tableData.conditionType;
        defaultValueIncrease = _tableData.conditionValueIncrease;
    }
}
