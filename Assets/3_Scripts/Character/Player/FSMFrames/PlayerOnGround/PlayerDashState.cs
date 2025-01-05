using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterControl.CanChangePlayer = false;
        anim.SetInteger("States", (int)PlayerEnums.STATES.DASH);
        characterControl.GetRigid.drag = 0f;
        characterControl.Dash();
        characterControl.GetRigid.useGravity= true;
        characterControl.CanTakeDamage = false;
    }

    public override void Execute()
    {
        characterControl.GroundCheck();
    }

    public override void FixedExecute()
    {
        characterControl.SetGravity();
        characterControl.ApplyGravityForce();
    }

    public override void Exit()
    {
        characterControl.CanTakeDamage = true;
        characterControl.GetRigid.useGravity = false;
        characterControl.CanChangePlayer = true;
    }
}
