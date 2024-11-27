using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    Animator anim = null;
    public PlayerDashState(CharacterCtrl _controller) : base(_controller)
    {
        anim = _controller.Anim ;
    }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = 0f;
        characterCtrl.Dash();
        anim.SetTrigger("Dash");
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
