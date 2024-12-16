using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    protected Rigidbody rigid = null;
    public PlayerOnAirState(WarriorMovementControl _controller) : base(_controller) 
    {
        rigid = _controller.GetRigid;
    }

    public override void Enter()
    {
        characterCtrl.GetRigid.drag = characterCtrl.AirDrag;
    }
}
