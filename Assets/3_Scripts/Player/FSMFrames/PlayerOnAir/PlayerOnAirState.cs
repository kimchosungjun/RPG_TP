using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    protected Rigidbody rigid = null;
    protected Animator anim = null;
    public PlayerOnAirState(WarriorMovementControl _controller) : base(_controller) 
    {
        rigid = _controller.GetRigid;
        anim = _controller.GetAnim;
    }

    public override void Enter()
    {
        characterCtrl.GetRigid.drag = characterCtrl.AirDrag;
        anim.SetFloat("VerticalVelocity", rigid.velocity.y);
    }
}
