using UnityEngine;
using PlayerTableClasses;

[CreateAssetMenu(fileName = "PlayerBuffSkillSO", menuName = "PlayerActionSOData/PlayerBuffSkillSO")]
public class PlayerConditionActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float[] actionMultipliers;
    [SerializeField] protected int[] attributeStatTypes;
    [SerializeField] protected int[] effectStatTypes;
    [SerializeField] protected int[] continuityTypes;
    [SerializeField] protected float[] defaultValues;
    public int GetBuffCnt() { return actionMultipliers.Length; }

    public float GetMultiplier(int _combo)
    {
        if (actionMultipliers.Length - 1 >= _combo)
            return actionMultipliers[_combo];
        return -1;
    }

    public int GetAttributeStatType(int _combo)
    {
        if (attributeStatTypes.Length - 1 >= _combo)
            return attributeStatTypes[_combo];
        return -1;
    }

    public int GetEffectStatType(int _combo)
    {
        if (effectStatTypes.Length - 1 >= _combo)
            return effectStatTypes[_combo];
        return -1;
    }

    public int GetContinuityType(int _combo)
    {
        if (continuityTypes.Length - 1 >= _combo)
            return continuityTypes[_combo];
        return -1;
    }

    public float GetDefaultValue(int _combo)
    {
        if (defaultValues.Length - 1 >= _combo)
            return defaultValues[_combo];
        return -1;
    }

    public void SetSOData(PlayerConditionSkillTableData _buffSkillTable)
    {
         actionName = _buffSkillTable.name;
         actionDescription = _buffSkillTable.description;
         actionMultipliers = _buffSkillTable.multipliers;
         actionParticleID = _buffSkillTable.particle;
        coolTime = _buffSkillTable.coolTime;
        maintainEffectTime = _buffSkillTable.effectMaintainTime;
        attributeStatTypes = _buffSkillTable.attributeStatTypes;
        effectStatTypes = _buffSkillTable.effectStatTypes;
        continuityTypes = _buffSkillTable.continuityTypes;
        defaultValues = _buffSkillTable.defaultValues;
    }
}
