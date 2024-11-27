using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerState
{
    Animator anim = null;
    public PlayerDash(CharacterCtrl _controller) : base(_controller)
    {
        anim = _controller.Anim ;
    }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = 0f;
        characterCtrl.Dash();
        anim.SetTrigger("Dash");
    }
}
