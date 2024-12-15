using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackSkillSO", menuName = "PlayerActionSOData/PlayerAttackSkillSO")]
public class PlayerAttackSkillActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float actionMultiplier;
    [SerializeField] protected int attackEffectType;

    public float GetActionMultiplier { get { return actionMultiplier; } }
    public int GetAttackEffectType { get { return attackEffectType; } }
    
    public void SetSOData(PlayerTable.PlayerAttackSkillTableData _attackSkillData)
    {
        actionName = _attackSkillData.name;
        actionDescription = _attackSkillData.description;
        actionParticleID = _attackSkillData.particle;
        coolTime = _attackSkillData.coolTime;
        maintainEffectTime = _attackSkillData.effectMaintainTime;

        actionMultiplier = _attackSkillData.multiplier;
        attackEffectType = _attackSkillData.effectType; 
    }
}