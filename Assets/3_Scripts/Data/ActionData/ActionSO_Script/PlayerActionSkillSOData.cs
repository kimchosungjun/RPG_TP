using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSkillSOData : PlayerBaseActionSOData
{
    [SerializeField] protected float coolTime;
    [SerializeField] protected float maintainEffectTime;
    public float GetCoolTime { get { return coolTime; } }
    public float GetMaintainEffectTime { get { return maintainEffectTime; } }
}
