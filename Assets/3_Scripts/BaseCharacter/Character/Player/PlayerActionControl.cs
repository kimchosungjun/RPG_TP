using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerActionControl : MonoBehaviour
{
    protected Animator anim = null;
    protected PlayerStat stat = null; 
    protected PlayerMovementControl movementControl = null;
    protected PlayerStatControl statControl = null;
    /// <summary>
    /// 공격, 버프에 사용할 플레이어 스탯 데이터가 필요
    /// </summary>
    public abstract void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl);

    public abstract void EscapeAttackState();

    #region Cool Down
    protected float normalAttackTime = 0;
    protected float SkillTime = 0;
    protected float UltimateTime = 0;
    public void NormalAttackCoolDown()
    {
        normalAttackTime = Time.time;
        movementControl.CanNormalAttack = false;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.GetJoystickUI.InputNormalAttack();
        Invoke("CoolDownNormal", 0.1f);
    }

    public void CoolDownNormal() 
    {
        movementControl.CanNormalAttack = true;
    }
    public void SkillCoolDown(float _time)
    {
        SkillTime = Time.time;
        movementControl.CanSkill = false;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.GetJoystickUI.InputSkill(_time);
        Invoke("CoolDownSkill", _time);
    }
    public void CoolDownSkill()
    {
        movementControl.CanSkill = true;
    }

    public void UltimateCoolDown(float _time)
    {
        UltimateTime = Time.time;   
        movementControl.CanUltimate = false;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.GetJoystickUI.InputUltimate(_time);
        Invoke("CoolDownUltimate", _time);
    }
    public void CoolDownUltimate()
    {
        movementControl.CanUltimate = true;
    }
    #endregion
}
