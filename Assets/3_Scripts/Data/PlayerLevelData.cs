using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLevelData 
{
    [SerializeField] protected List<int> normalLevelupGold;
    [SerializeField] protected List<int> skillLevelupGold;
    [SerializeField] protected List<int> ultimateSkillLevelupGold;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected List<int> needExp;

    public PlayerLevelData(int maxLevel, List<int> needExp, List<int> normalLevelupGold, List<int> skillLevelupGold, List<int> ultimateSkillLevelupGold)
    {
        this.maxLevel = maxLevel;
        this.needExp = needExp;
        this.normalLevelupGold = normalLevelupGold;
        this.skillLevelupGold = skillLevelupGold;
        this.ultimateSkillLevelupGold = ultimateSkillLevelupGold;
    }
}
// 1에서 만렙이 20까지라면?
// 19번의 레벨업 데이터가 필요
[System.Serializable]
public class PlayerLevelDataWrapper
{
    public PlayerLevelData[] PlayerLevelData;
}