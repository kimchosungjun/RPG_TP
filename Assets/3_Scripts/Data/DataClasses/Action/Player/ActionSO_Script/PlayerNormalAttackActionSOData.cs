using UnityEngine;
using PlayerTableClassGroup;
using PlayerEnums;
using System;

/// <summary>
/// Normal Attack
/// </summary>
[CreateAssetMenu(fileName ="PlayerNormalAttackSO",menuName = "PlayerActionSOData/PlayerNormalAttackSO")]
public class PlayerNormalAttackActionSOData : PlayerBaseActionSOData
{
    [SerializeField] protected float[] effectMaintatinTimes;
    [SerializeField] protected int[] attackEffectType;
    [SerializeField] protected float[] actionMultipliers;

    TYPEIDS normalType;

    public float GetMaintainTime(int _combo)
    {
        if (effectMaintatinTimes.Length - 1 >= _combo)
            return effectMaintatinTimes[_combo];
        return -1;
    }

    public int GetAttackEffectType(int _combo)
    {
        if (attackEffectType.Length - 1 >= _combo)
            return attackEffectType[_combo];
        return -1;
    }

    public float GetActionMultiplier(int _combo)
    {
        if (actionMultipliers.Length - 1 >= _combo)
            return actionMultipliers[_combo];
        return -1f;
    }

    public void SetSOData(PlayerNormalAttackTableData _normalAttackData, TYPEIDS _typeID)
    {
        normalType = _typeID;
        actionName = _normalAttackData.name;
        actionDescription = _normalAttackData.description;
        actionParticleID = _normalAttackData.particle;
        effectMaintatinTimes = _normalAttackData.effectMaintainTimes;
        attackEffectType = _normalAttackData.effects;
        actionMultipliers = _normalAttackData.multipliers;
    }

    public override void LevelUp()
    {
        currentLevel += 1;
        SetSOData(SharedMgr.TableMgr.GetPlayer.GetPlayerNormalAttackData(normalType, currentLevel), normalType);
    }

    public override Tuple<string, int, string> GetNextLevelData()
    {
        PlayerNormalAttackTableData data = SharedMgr.TableMgr.GetPlayer.GetPlayerNormalAttackData(normalType, currentLevel + 1);
        Tuple<string, int, string> result = new Tuple<string, int, string>(data.name, currentLevel + 1, data.description);
        return result;
    }

    public override int GetNextLevelUpGold()
    {
        if (currentLevel == maxLevel) return -1;
        return SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().normalAttackLevelupGolds[currentLevel - 1];
    }
}