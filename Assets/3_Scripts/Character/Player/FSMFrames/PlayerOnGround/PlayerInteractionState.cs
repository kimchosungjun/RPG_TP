using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionState : PlayerOnGroundState
{
    public PlayerInteractionState(PlayerMovementControl _controller) : base(_controller)  {  }

    public override void Enter()
    {
        base.Enter();
        anim.SetInteger("States", (int)PlayerEnums.STATES.INTERACTION);
    }

}
