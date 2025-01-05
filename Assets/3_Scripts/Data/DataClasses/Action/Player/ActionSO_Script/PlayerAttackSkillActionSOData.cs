using UnityEngine;
using PlayerTableClassGroup;

[CreateAssetMenu(fileName = "PlayerAttackSkillSO", menuName = "PlayerActionSOData/PlayerAttackSkillSO")]
public class PlayerAttackSkillActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float actionMultiplier;
    [SerializeField] protected int attackEffectType;

    public float GetActionMultiplier { get { return actionMultiplier; } }
    public int GetAttackEffectType { get { return attackEffectType; } }
    
    public void SetSOData(PlayerAttackSkillTableData _attackSkillData)
    {
        actionName = _attackSkillData.name;
        actionDescription = _attackSkillData.description;
        actionParticleID = _attackSkillData.particle;
        coolTime = _attackSkillData.coolTime;
        maintainEffectTime = _attackSkillData.effectMaintainTime[0];
        actionMultiplier = _attackSkillData.multiplier[0];
        attackEffectType = _attackSkillData.effectType[0]; 
    }
}