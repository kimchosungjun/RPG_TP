using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        // no longer interact any actions : attack, buff .. ect
        anim.SetInteger("States", (int)PlayerEnums.STATES.DEATH);
        SharedMgr.EnvironmentMgr.GetPlayerCtrl.GetPlayer.SetNoneInteractionType();
    }

    public override void Exit()
    {
        SharedMgr.EnvironmentMgr.GetPlayerCtrl.GetPlayer.SetCharacterType();
    }
}
