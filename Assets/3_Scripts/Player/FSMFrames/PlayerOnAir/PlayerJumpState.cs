using UnityEngine;
using PlayerEnums;
/// <summary>
/// 위로 힘 가하기는 Y축 속도와 땅 체크 문제로 키가 눌리는 시점에 구현함
/// </summary>
public class PlayerJumpState : PlayerOnAirState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Guarantee Jump State (0.1f)

    float time = 0f;
    float jumpTimer = 0.1f;
    bool onceReset = true;
    bool maintainJumpState = true;

    public PlayerJumpState(WarriorMovement _controller) : base(_controller) { }

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
        anim.SetBool("IsOnGround", false);
        anim.SetTrigger("Jump");
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

    public void CheckTransitionState()
    {
        if (characterCtrl.IsOnGround)
        {
            characterCtrl.ChangeState(PlayerEnums.STATES.MOVEMENT);
        }
        else
        {
            if (rigid.velocity.y < 0)
            {
                characterCtrl.ChangeState(PlayerEnums.STATES.FALL);
            }
        }
    }
    #endregion
}
