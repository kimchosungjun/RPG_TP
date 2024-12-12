using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBuffSkillSO", menuName = "PlayerActionSOData/PlayerBuffSkillSO")]
public class PlayerBuffActionSOData : PlayerBaseActionSOData
{
    [SerializeField] protected float coolTime;
    [SerializeField] protected float maintainEffectTime;
    [SerializeField] protected int[] useStatTypes;
    [SerializeField] protected int[] effectStatTypes;
    [SerializeField] protected int[] continuityTypes;
    
    public float GetCoolTime { get { return coolTime; } }
    public float GetMaintainEffectTime { get {return maintainEffectTime; } }    
    
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
}
