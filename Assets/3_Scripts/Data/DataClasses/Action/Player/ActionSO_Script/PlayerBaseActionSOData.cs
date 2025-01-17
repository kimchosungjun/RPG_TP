using System;
using UnityEngine;

/// <summary>
/// 플레이어의 공격, 버프의 부모
/// </summary>
public class PlayerBaseActionSOData : ScriptableObject
{
    #region Protected
    [SerializeField] protected string actionName;
    [SerializeField] protected string actionDescription;
    [SerializeField] protected int actionParticleID;
    [SerializeField] protected int currentLevel;
    [SerializeField] protected int maxLevel;
    #endregion

    #region Public Property & Method

    public string GetActionName { get { return actionName; } }
    public string GetActionDescription { get { return actionDescription; } }
    public int GetParticleID() { return actionParticleID; }
    public int GetCurrentLevel { get { return currentLevel; } }
    public int GetMaxLevel { get { return maxLevel; } }

    public virtual void LevelUp() { }
    public bool CanLevelUp() { if (currentLevel >= maxLevel) return false; return true; }

    /// <summary>
    /// Name, Lv, Description
    /// </summary>
    /// <returns></returns>
    public virtual Tuple<string,int,string> GetNextLevelData() { return null;  }
    public virtual int GetNextLevelUpGold()  { return 0; }

    public void SetLevelData(int _curLv, int _maxLv) { currentLevel = _curLv; maxLevel = _maxLv; }
    #endregion

}


