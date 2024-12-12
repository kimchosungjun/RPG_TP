using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    protected Rigidbody rigid = null;
    protected Animator anim = null;
    public PlayerOnAirState(WarriorMovement _controller) : base(_controller) 
    {
        rigid = _controller.Rigid;
        anim = _controller.Anim;
    }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = characterCtrl.AirDrag;
        anim.SetFloat("VerticalVelocity", rigid.velocity.y);
    }
}
