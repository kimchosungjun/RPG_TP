using UnityEngine;
using PlayerTableClassGroup;
using PlayerEnums;
using System;

[CreateAssetMenu(fileName = "PlayerBuffSkillSO", menuName = "PlayerActionSOData/PlayerBuffSkillSO")]
public class PlayerConditionActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float[] actionMultipliers;
    [SerializeField] protected int[] attributeStatTypes;
    [SerializeField] protected int[] effectStatTypes;
    [SerializeField] protected int[] continuityTypes;
    [SerializeField] protected float[] defaultValues;
    [SerializeField] protected int[] applyType;
    [SerializeField] protected int[] partyType;

    CONDITION_SKILLS skillType;

    public int GetBuffCnt() { return actionMultipliers.Length; }

    public float GetMultiplier(int _combo)
    {
        float result = 0f;
        if (applyType.Length - 1 >= _combo)
        {
            switch (applyType[_combo])
            {
                case (int)EffectEnums.CONDITION_APPLY_TYPE.VALUE:
                    if (actionMultipliers.Length - 1 >= _combo)
                        result = actionMultipliers[_combo];
                    break;
                case (int)EffectEnums.CONDITION_APPLY_TYPE.OWN_PERCENT:
                    result = GetDefaultValue(_combo);
                    break;
                default:
                    break;
            }
        }
        return result;
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
        return 0;
    }

    public int GetApplyType(int _combo)
    {
        if (applyType.Length - 1 >= _combo)
            return applyType[_combo];
        return -1;
    }

    public int GetPartyType(int _combo)
    {
        if (partyType.Length - 1 >= _combo)
            return partyType[_combo];
        return -1;
    }

    public void SetSOData(PlayerConditionSkillTableData _buffSkillTable, CONDITION_SKILLS _skillType)
    {
        skillType = _skillType;
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
        applyType = _buffSkillTable.applyType;
        partyType = _buffSkillTable.partyType;
        isUltimateSkill = _buffSkillTable.isUltimate;
    }

    public override void LevelUp()
    {
        currentLevel += 1;
        SetSOData(SharedMgr.TableMgr.GetPlayer.GetPlayerBuffSkillTableData(skillType, currentLevel), skillType);
    }

    public override Tuple<string, int, string> GetNextLevelData()
    {
        PlayerConditionSkillTableData data = SharedMgr.TableMgr.GetPlayer.GetPlayerBuffSkillTableData(skillType, currentLevel + 1);
        Tuple<string, int, string> result = new Tuple<string, int, string>(data.name, currentLevel + 1, data.description);
        return result;
    }

    public override int GetNextLevelUpGold()
    {
        if (currentLevel == maxLevel) return -1;

        if (isUltimateSkill)
        {
            return SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().ultimateLevelupGolds[currentLevel - 1];
        }
        else
        {
            return SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().skillLevelupGolds[currentLevel - 1];
        }
    }
}
