using UnityEngine;

public class PlayerSkillState : PlayerActionState
{
    public PlayerSkillState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo) { }
    public override void Enter() 
    {
        base.Enter();
        anim.SetInteger("States", (int)PlayerEnums.STATES.SKILL);
    }

    /// <summary>
    /// 기본공격 콤보 사이에 끼어 쓸 수 있게 시간 초기화
    /// </summary>
    public override void Exit() 
    {
        base.Exit();
        attackCombo.SetComboTime();
    }
}
