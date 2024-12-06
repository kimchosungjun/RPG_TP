using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "PlayerAttackAction", menuName = "Player/Action/Attack")]
public class PlayerAttackActionData : PlayerActionData
{
    [Header("공격")]
    [SerializeField, Tooltip("데미지 비율")] protected float[] damageMultipliers;
    [SerializeField, Tooltip("공격의 효과")] protected E_ATTACK_EFFECT_TYPES[] attackEffectTypes;

    // 0번 인덱스부터 시작하므로 -1을 꼭 해줘야 한다.
    public string GetActionDescription(int _level) { return actionDescriptions[_level - 1]; }
    public float GetDamageMultiplier(int _level) { return damageMultipliers[_level - 1]; }
    public E_ATTACK_EFFECT_TYPES GetAttackEffect(int _level) { return attackEffectTypes[_level - 1]; }
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