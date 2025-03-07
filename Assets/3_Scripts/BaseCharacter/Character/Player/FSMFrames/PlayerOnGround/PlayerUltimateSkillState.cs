public class PlayerUltimateSkillState : PlayerActionState
{
    public PlayerUltimateSkillState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo) { }

    public override void Enter()
    {
        base.Enter();
        anim.SetInteger("States", (int)PlayerEnums.STATES.ULTIMATESKILL);
        // 플레이어 Ctrl에게 궁극기 스킬 정보를 받아 스킬 실행
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
