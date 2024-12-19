using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(WarriorMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterCtrl.GetRigid.drag = 0f;
        characterCtrl.Dash();
        anim.SetInteger("States", (int)PlayerEnums.STATES.DASH);
    }

    public override void Execute()
    {
        characterCtrl.GroundCheck();
    }

    public override void FixedExecute()
    {
        characterCtrl.SetGravity();
        characterCtrl.ApplyGravityForce();
    }
}
