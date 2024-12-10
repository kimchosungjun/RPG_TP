using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "PlayerAttackAction", menuName = "Player/Action/Attack")]
public class PlayerAttackActionData : PlayerActionData
{
    [Header("공격")]
    [SerializeField, Tooltip("데미지 비율")] protected float[] damageMultipliers;
    [SerializeField, Tooltip("공격의 효과")] protected ATTACK_EFFECT_TYPES[] attackEffectTypes;

    // 0번 인덱스부터 시작하므로 -1을 꼭 해줘야 한다.
    public float GetDamageMultiplier(int _level) { return damageMultipliers[_level - 1]; }
    public ATTACK_EFFECT_TYPES GetAttackEffect(int _level) { return attackEffectTypes[_level - 1]; }
}