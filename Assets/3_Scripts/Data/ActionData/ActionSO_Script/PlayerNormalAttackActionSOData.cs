using UnityEngine;
using PlayerTableClasses;

/// <summary>
/// Normal Attack
/// </summary>
[CreateAssetMenu(fileName ="PlayerNormalAttackSO",menuName = "PlayerActionSOData/PlayerNormalAttackSO")]
public class PlayerNormalAttackActionSOData : PlayerBaseActionSOData
{
    [SerializeField] protected float[] effectMaintatinTimes;
    [SerializeField] protected int[] attackEffectType;
    [SerializeField] protected float[] actionMultipliers;
  
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

    public void SetSOData(PlayerNormalAttackTableData _normalAttackData)
    {
        actionName = _normalAttackData.name;
        actionDescription = _normalAttackData.description;
        actionParticleID = _normalAttackData.particle;
        effectMaintatinTimes = _normalAttackData.effectMaintainTimes;
        attackEffectType = _normalAttackData.effects;
        actionMultipliers = _normalAttackData.multipliers;
    }
}