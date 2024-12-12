using UnityEngine;

/// <summary>
/// Normal Attack
/// </summary>
[CreateAssetMenu(fileName ="PlayerNormalAttackSO",menuName = "PlayerActionSOData/PlayerNormalAttackSO")]
public class PlayerNormalAttackActionSOData : PlayerBaseActionSOData
{
    [SerializeField] protected int[] attackEffectType;
    [SerializeField] protected float[] actionMultipliers;
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

    public void SetSOData(PlayerTable.PlayerNormalAttackTableData _normalAttackData)
    {
        actionName = _normalAttackData.name;
        actionDescription = _normalAttackData.description;
        actionParticleID = _normalAttackData.particle;

        attackEffectType = _normalAttackData.effects;
        actionMultipliers = _normalAttackData.multipliers;
    }
}