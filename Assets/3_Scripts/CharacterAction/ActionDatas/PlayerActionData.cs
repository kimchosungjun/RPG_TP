using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PlayerActionData : ScriptableObject
{
    #region Protect 
    [SerializeField, Tooltip("필요한 파티클 종류")] protected E_PARTICLES actionParticleKey = E_PARTICLES.NONE;

    [Header("행동 설명")]
    [SerializeField, Tooltip("행동 이름")] protected string actionName;
    [SerializeField, Tooltip("행동 설명")] protected string[] actionDescriptions;

    [Header("지속 시간")]
    [SerializeField, Tooltip("행동 지속시간(버프, 공격 효과)")] protected float[] actionMaintainTimers;
    [SerializeField, Tooltip("행동 쿨타임")] protected float[] coolTimers;
    protected float lastDoTime = -100;
    #endregion

    #region Public : Method & Property
    public E_PARTICLES ActionParticleKey { get { return actionParticleKey; } }
    public string GetActionName() { return actionName; } 
    public float GetCoolTimer(int _level)  { return coolTimers[_level]; }
    public float GetActionMaintainTimer(int _level) { return coolTimers[_level]; }
    #endregion 

    public float CheckCoolTime(int _level) 
    {
        float calculateTime = (Time.time - lastDoTime);
        return (coolTimers[_level] < calculateTime) ? -1 : calculateTime; 
    }

    /// <summary>
    /// 쿨타임 체크를 위해 행동 실행시 반드시 호출
    /// </summary>
    public void DoAction() { lastDoTime = Time.time; }
} 


