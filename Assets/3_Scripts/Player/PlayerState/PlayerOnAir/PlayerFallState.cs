using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerOnAirState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    public PlayerFallState(CharacterCtrl _controller) : base(_controller) { }

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame

    public override void Execute()
    {
        characterCtrl.LimitMovementSpeed();
        characterCtrl.GroundCheck();
        InputKey();
        CheckTransitionMovementState();
    }

    public override void FixedExecute()
    {
        characterCtrl.SetMoveDirection();
        characterCtrl.AirBlock();
        characterCtrl.SetGravity();
        characterCtrl.SetRotation();
        characterCtrl.ApplyAirForce();
        characterCtrl.ApplyAirRotation();
    }

    #endregion

    /******************************************/
    /********** 업데이트 메서드 ************/
    /******************************************/

    #region Execute
    public void InputKey()
    {
        characterCtrl.XMove = Input.GetAxisRaw("Horizontal");
        characterCtrl.ZMove = Input.GetAxisRaw("Vertical");
    }

    public void CheckTransitionMovementState()
    {
        if (characterCtrl.IsOnGround)
            characterCtrl.ChangeState(E_PLAYER_FSM.MOVEMENT);
    }
    #endregion
}
