
public class PlayerActionState : PlayerOnGroundState
{
    protected PlayerAttackCombo attackCombo = null;
    public PlayerActionState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller) { attackCombo = _attackCombo; }

    public override void Enter()
    {
        base.Enter();
        anim.applyRootMotion = true;
    }

    public override void Exit()
    {
        base.Exit();
        anim.applyRootMotion = false;
        characterControl.SetMoveRotation = characterControl.transform.rotation;
        attackCombo.SetComboTime();
    }
}
