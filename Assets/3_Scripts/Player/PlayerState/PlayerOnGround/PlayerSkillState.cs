using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerAttackState
{
    public PlayerSkillState(CharacterMovement _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo) { }
    public override void Enter() 
    {
        anim.SetBool("IsAttackEnd", false);
        anim.SetFloat("PlaneVelocity", 0f);
        anim.SetTrigger("Skill");

        // 플레이어 Ctrl에게 스킬 정보를 받아 스킬 실행
    }
    /// <summary>
    /// 기본공격 콤보 사이에 끼어 쓸 수 있게 시간 초기화
    /// </summary>
    public override void Exit() { attackCombo.SetComboTime(); }
}
