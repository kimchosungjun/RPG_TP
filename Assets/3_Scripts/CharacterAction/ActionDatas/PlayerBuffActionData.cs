using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "PlayerBuffAction", menuName = "Player/Action/Buff")]
public class PlayerBuffActionData : PlayerActionData
{
    [Header("버프")]
    [SerializeField, Tooltip("버프 효과")] protected E_BUFF_APPLY_STAT[] applyStatTypes;
    [SerializeField, Tooltip("어떤 스탯이 버프에 효과를 주는지")] protected E_BUFF_EFFECT_STAT[] effectStatTypes;
    [SerializeField, Tooltip("버프 지속성")] protected E_BUFF_COUNTINUITY[] continuityTypes;
    [SerializeField, Tooltip("버프 비율")] protected float[] buffMultipliers;

    // 0번 인덱스부터 시작하므로 -1을 꼭 해줘야 한다.
    public string GetActionDescription(int _level) { return actionDescriptions[_level - 1]; }
    public float GetBuffMultiplier(int _level) { return buffMultipliers[_level - 1]; }
    public E_BUFF_APPLY_STAT GetBuffApplyStat(int _level) { return applyStatTypes[_level - 1]; }
    public E_BUFF_EFFECT_STAT GetBuffEffectStat(int _level) { return effectStatTypes[_level - 1]; }
    public E_BUFF_COUNTINUITY GetBuffContinuityStat(int _level) { return continuityTypes[_level - 1]; }
    public List<string> GetUpgradeDescriptions(int _level)
    {
        List<string> descriptions = new List<string>();
        int cnt = actionDescriptions.Length;
        string currentDescription = "";
        string nextDescription = "";

        if (cnt <= _level)
        {
            currentDescription = actionDescriptions[_level - 1];
            nextDescription = "더 이상 강화할 수 없습니다.";
        }
        else
        {
            currentDescription = actionDescriptions[_level - 1];
            nextDescription = actionDescriptions[_level];
        }
        descriptions.Add(currentDescription);
        descriptions.Add(nextDescription);
        return descriptions;
    }
}