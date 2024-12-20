using MonsterTableClasses;
using UnityEngine;

[System.Serializable]
public class MonsterConditionActionData : MonsterBaseActionData
{
    [SerializeField] protected int conditionType;
    [SerializeField] protected float defaultValueIncrease;
    [SerializeField] protected int startLevel;

    public int GetConditionType { get { return conditionType; } }   
    public MonsterConditionActionData() { }
    public void SetConditionData(MonsterConditionTableData _tableData, int _level)
    {
        attribute = _tableData.attribute;
        multiplier = _tableData.multiplier;
        effect = _tableData.effect;
        maintainTime = _tableData.maintainTime; 
        coolTime = _tableData.coolTime;
        startLevel = _tableData.startLevel;
        conditionType = _tableData.conditionType;
        defaultValueIncrease = _tableData.conditionValueIncrease;
        defaultValue = _tableData.defaultConditionValue + (_level-startLevel)*defaultValueIncrease ;
    }
}
