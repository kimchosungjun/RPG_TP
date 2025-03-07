using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerOnGroundState : PlayerState
{
    public PlayerOnGroundState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterControl.GetRigid.drag = characterControl.GroundDrag;
    }
}
