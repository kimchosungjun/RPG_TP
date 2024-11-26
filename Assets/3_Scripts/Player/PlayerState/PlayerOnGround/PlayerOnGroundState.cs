using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerOnGroundState : PlayerState
{
    public PlayerOnGroundState(CharacterCtrl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterCtrl.Rigid.drag = characterCtrl.GroundDrag;
    }
}
