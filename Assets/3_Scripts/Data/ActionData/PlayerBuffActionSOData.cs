using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBuffSkillSO", menuName = "PlayerActionSOData/PlayerBuffSkillSO")]
public class PlayerBuffActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float[] actionMultipliers;
    [SerializeField] protected int[] useStatTypes;
    [SerializeField] protected int[] effectStatTypes;
    [SerializeField] protected int[] continuityTypes;
    

    
    public int GetUseStatType(int _combo)
    {
        if (useStatTypes.Length - 1 >= _combo)
            return useStatTypes[_combo];
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

    public void SetSOData(PlayerTable.PlayerBuffSkillTableData _buffSkillTable)
    {
         actionName = _buffSkillTable.name;
         actionDescription = _buffSkillTable.description;
         actionMultipliers = _buffSkillTable.multipliers;
         actionParticleID = _buffSkillTable.particle;

        coolTime = _buffSkillTable.coolTime;
        maintainEffectTime = _buffSkillTable.effectMaintainTime;
        useStatTypes = _buffSkillTable.useStatTypes;
        effectStatTypes = _buffSkillTable.effectStatTypes;
        continuityTypes = _buffSkillTable.continuityTypes;
    }
}
