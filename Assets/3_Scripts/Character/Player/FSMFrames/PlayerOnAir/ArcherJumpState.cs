using UnityEngine;
using PlayerEnums;

public class ArcherJumpState : PlayerJumpState
{
    public ArcherJumpState(PlayerMovementControl _controller) : base(_controller) { }

    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Double Jump
    bool isDoubleJump = false;
    #endregion

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame
    public override void Enter()
    {
        // 저항 값 변경
        base.Enter();
        // 애니메이션 & 점프 상태 변수 초기화
        time = 0f;
        onceReset = true;
        maintainJumpState = true;
        characterControl.IsOnGround = false;
        anim.SetInteger("States", (int)STATES.JUMP);
    }

    public override void Execute()
    {
        if (onceReset && time > jumpTimer)
        {
            onceReset = false;
            maintainJumpState = false;
        }
        else
            time += Time.deltaTime;

        characterControl.MonsterCheck();
        if (maintainJumpState == true) return;
        characterControl.LimitMovementSpeed();
        characterControl.GroundCheck();
        InputKey();
        CheckTransitionState();
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


    public override void InputKey()
    {
        base.InputKey();
        if (Input.GetKeyDown(KeyCode.Space) && isDoubleJump == false) 
        {
            isDoubleJump = true;
            characterControl.AddforceForJump();
        }
    }

    public override void Exit()
    {
        base.Exit();
        isDoubleJump = false;
    }
    #endregion


}
