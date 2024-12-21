using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerState
{
    public PlayerHitState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        base.Enter();
        anim.SetInteger("States", (int)PlayerEnums.STATES.HIT);
    }
}
