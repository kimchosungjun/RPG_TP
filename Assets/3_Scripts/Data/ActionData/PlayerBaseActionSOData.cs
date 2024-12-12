using UnityEngine;

/// <summary>
/// 플레이어의 공격, 버프의 부모
/// </summary>
public class PlayerBaseActionSOData : ScriptableObject
{
    #region Protected
    
    [SerializeField] protected string actionName;
    [SerializeField] protected string actionDescription;
    [SerializeField] protected float[] actionMultipliers;
    [SerializeField] protected int actionParticleID;

    #endregion

    #region Public Property & Method

    public string GetActionName { get { return actionName; } }
    public string GetActionDescription { get { return actionDescription; } }
    public float GetActionMultiplier(int _combo)
    {
        if (actionMultipliers.Length-1 >=_combo)
            return actionMultipliers[_combo];
        return -1f;
    }

    // To Do ~~~~~~~ 
    // 나중에 수정할 것
    /// <summary>
    /// 추후에 Enum값으로 변환해서 반환 or int 값을 enum값으로 파싱
    /// </summary>
    /// <returns></returns>
    public int GetParticleID() { return actionParticleID; }
    
    #endregion
}


