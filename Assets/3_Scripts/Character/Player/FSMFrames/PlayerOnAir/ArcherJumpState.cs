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
        characterCtrl.IsOnGround = false;
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

        characterCtrl.MonsterCheck();
        if (maintainJumpState == true) return;
        characterCtrl.LimitMovementSpeed();
        characterCtrl.GroundCheck();
        InputKey();
        CheckTransitionState();
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


    public override void InputKey()
    {
        base.InputKey();
        if (Input.GetKeyDown(KeyCode.Space) && isDoubleJump == false) 
        {
            isDoubleJump = true;
            characterCtrl.AddforceForJump();
        }
    }

    public override void Exit()
    {
        base.Exit();
        isDoubleJump = false;
    }
    #endregion


}
