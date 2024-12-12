using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackSkillSO", menuName = "PlayerActionSOData/PlayerAttackSkillSO")]
public class PlayerAttackSkillActionSOData : PlayerAttackActionSOData
{
    [SerializeField] protected float coolTime;
    [SerializeField] protected float effectMaintainTime;

    public float GetCoolTime { get { return coolTime; } }   
    public float GetEffectMaintainTime { get {return effectMaintainTime; } }
}