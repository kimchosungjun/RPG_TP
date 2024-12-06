using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionData : ScriptableObject
{
    [TextArea(5,10), SerializeField, Header("행동 설명")] protected string actionDescription;
    [SerializeField, Tooltip("필요한 파티클 종류")] protected E_PARTICLES actionParticleKey = E_PARTICLES.NONE;
}

public class AttackActionData : ActionData
{
    //protected int[] 
}