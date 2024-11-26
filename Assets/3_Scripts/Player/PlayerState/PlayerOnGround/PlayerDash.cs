using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerOnGroundState
{
    Animator anim = null;
    public PlayerDash(CharacterCtrl _controller) : base(_controller)
    {
        anim = _controller.Anim ;
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetTrigger("Dash");
    }
}
