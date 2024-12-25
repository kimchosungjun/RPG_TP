using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    protected Rigidbody rigid = null;
    public PlayerOnAirState(PlayerMovementControl _controller) : base(_controller) 
    {
        rigid = _controller.GetRigid;
    }

    public override void Enter()
    {
        characterControl.GetRigid.drag = characterControl.AirDrag;
        characterControl.IsOnAirState = true;
    }

    public override void Exit()
    {
        base.Exit();
        characterControl.IsOnAirState = false;
    }
}
