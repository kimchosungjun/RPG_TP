using UnityEngine;

/// <summary>
/// Normal Attack
/// </summary>
[CreateAssetMenu(fileName ="PlayerNormalAttackSO",menuName = "PlayerActionSOData/PlayerNormalAttackSO")]
public class PlayerAttackActionSOData : PlayerBaseActionSOData
{
    [SerializeField] protected int[] attackEffectType; 

    public int GetAttackEffectType(int _combo)
    {
        if (attackEffectType.Length - 1 >= _combo)
            return attackEffectType[_combo];
        return -1;
    }
}