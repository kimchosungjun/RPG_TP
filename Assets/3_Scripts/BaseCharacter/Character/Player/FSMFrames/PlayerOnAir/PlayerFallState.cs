using UnityEngine;
using PlayerEnums;

public class PlayerFallState : PlayerOnAirState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    public PlayerFallState(PlayerMovementControl _controller) : base(_controller) { }

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame

    public override void Enter()
    {
        base.Enter();
        anim.SetInteger("States", (int)STATES.FALL);
    }

    public override void Execute()
    {
        characterControl.MonsterCheck(); 
        characterControl.LimitMovementSpeed();
        characterControl.GroundCheck();
        InputKey();
        CheckTransitionMovementState();
    }

    public override void FixedExecute()
    {
        characterControl.SetMoveDirection();
        characterControl.AirBlock();
        characterControl.SetGravity();
        characterControl.SetRotation();
        characterControl.ApplyAirForce();
        characterControl.ApplyAirRotation();
    }

    #endregion

    /******************************************/
    /********** 업데이트 메서드 ************/
    /******************************************/

    #region Execute
    public void InputKey()
    {
        characterControl.XMove = Input.GetAxisRaw("Horizontal");
        characterControl.ZMove = Input.GetAxisRaw("Vertical");
    }

    public void CheckTransitionMovementState()
    {
        if (characterControl.IsOnGround)
            characterControl.ChangeState(STATES.MOVEMENT);
    }
    #endregion
}
