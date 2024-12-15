using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerOnGroundState : PlayerState
{
    public PlayerOnGroundState(WarriorMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterCtrl.GetRigid.drag = characterCtrl.GroundDrag;
    }
}
