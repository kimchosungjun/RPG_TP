using UnityEngine;
using PlayerTableClassGroup;
using PlayerEnums;
using System;

[CreateAssetMenu(fileName = "PlayerAttackSkillSO", menuName = "PlayerActionSOData/PlayerAttackSkillSO")]
public class PlayerAttackSkillActionSOData : PlayerActionSkillSOData
{
    [SerializeField] protected float actionMultiplier;
    [SerializeField] protected int attackEffectType;

    ATTACK_SKILLS skillType;

    public float GetActionMultiplier { get { return actionMultiplier; } }
    public int GetAttackEffectType { get { return attackEffectType; } }
    
    public void SetSOData(PlayerAttackSkillTableData _attackSkillData, ATTACK_SKILLS _skillType)
    {
        skillType = _skillType;
        actionName = _attackSkillData.name;
        actionDescription = _attackSkillData.description;
        actionParticleID = _attackSkillData.particle;
        coolTime = _attackSkillData.coolTime;
        maintainEffectTime = _attackSkillData.effectMaintainTime[0];
        actionMultiplier = _attackSkillData.multiplier[0];
        attackEffectType = _attackSkillData.effectType[0];
        isUltimateSkill = _attackSkillData.isUltimate;
    }

    public override void LevelUp()
    {
        currentLevel += 1;
        if (SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat((int)playerTypeID) == null) return;
        if(isUltimateSkill)
            SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat((int)playerTypeID).GetSaveStat.currentUltimateSkillLevel = currentLevel;
        else
            SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat((int)playerTypeID).GetSaveStat.currentSkillLevel= currentLevel;

        SetSOData(SharedMgr.TableMgr.GetPlayer.GetPlayerAttackSkillTableData(skillType, currentLevel), skillType);
    }

    public override Tuple<string, int, string> GetNextLevelData()
    {
        PlayerAttackSkillTableData data = SharedMgr.TableMgr.GetPlayer.GetPlayerAttackSkillTableData(skillType, currentLevel + 1);
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