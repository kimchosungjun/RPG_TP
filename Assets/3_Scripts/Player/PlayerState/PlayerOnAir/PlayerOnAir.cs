using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAir : PlayerState
{
    protected Rigidbody rigid = null;
    protected Animator anim = null;

    public PlayerOnAir(CharacterCtrl _controller) : base(_controller) 
    {
        rigid = _controller.Rigid;
        anim = _controller.Anim;
    }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = characterCtrl.AirDrag;
        characterCtrl.SetVerticalVelocityAnimation();
    }
}
