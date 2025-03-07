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

    protected float time = 0f;
    protected float jumpTimer = 0.1f;
    protected bool onceReset = true;
    protected bool maintainJumpState = true;

    public PlayerJumpState(PlayerMovementControl _controller) : base(_controller) { }

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

    #endregion

    /******************************************/
    /********** 업데이트 메서드 ************/
    /******************************************/

    #region Execute
    public virtual void InputKey()
    {
        characterControl.XMove = Input.GetAxisRaw("Horizontal");
        characterControl.ZMove = Input.GetAxisRaw("Vertical");
    }

    public void CheckTransitionState()
    {
        if (characterControl.IsOnGround)
        {
            characterControl.ChangeState(STATES.MOVEMENT);
        }
        else
        {
            if (rigid.velocity.y < 0)
            {
                characterControl.ChangeState(STATES.FALL);
            }
        }
    }
    #endregion
}
