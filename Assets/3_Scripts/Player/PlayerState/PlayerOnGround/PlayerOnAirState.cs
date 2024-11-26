using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    public PlayerOnAirState(CharacterCtrl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = characterCtrl.AirDrag;
    }
}
