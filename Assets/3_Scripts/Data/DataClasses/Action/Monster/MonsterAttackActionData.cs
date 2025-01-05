using UnityEngine;
using MonsterTableClassGroup;

[System.Serializable]
public class MonsterAttackActionData : MonsterBaseActionData
{
    [SerializeField] protected float defaultValueIncrease;
    [SerializeField] protected int startLevel;
    public MonsterAttackActionData() { }

    public void SetConditionData(MonsterAttackTableData _tableData, int _level)
    {
        attribute = _tableData.attribute;
        multiplier = _tableData.multiplier;
        effect = _tableData.effect;
        maintainTime = _tableData.maintainTime;
        coolTime = _tableData.coolTime;
        defaultValueIncrease = _tableData.damageIncrease;
        startLevel = _tableData.startLevel;
        defaultValue = _tableData.defaultDamage + defaultValueIncrease*(_level-startLevel);
    }
}

#region 미사용 생성자
//public MonsterAttackActionData(MonsterAttackTableData _tableData)
//{
//    attribute = _tableData.attribute;
//    multiplier = _tableData.multiplier;
//    effect = _tableData.effect;
//    maintainTime = _tableData.maintainTime;
//    coolTime = _tableData.coolTime;
//    defaultValue = _tableData.defaultDamage;
//    defaultValueIncrease = _tableData.damageIncrease;
//}
#endregion