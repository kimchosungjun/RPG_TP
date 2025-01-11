using System.Collections;
using UnityEngine;
using PlayerEnums;
public class PlayerGroundMoveState : PlayerOnGroundState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Createor & Value
    Rigidbody rigid = null;
    public PlayerGroundMoveState(PlayerMovementControl _controller) : base(_controller) 
    {
        this.rigid = _controller.GetRigid;
    }
    #endregion

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame

    public override void Enter()
    {
        base.Enter();
        anim.SetInteger("States", (int)STATES.MOVEMENT);
    }

    public override void Execute()
    {
        characterControl.LimitMovementSpeed();
        characterControl.GroundCheck();
        SetPlaneVelocityAnimation();
        CheckTransitionFallState();
        InputKey();
    }

    public override void FixedExecute()
    {
        characterControl.SetMoveDirection();
        characterControl.SetSlopeMoveDirection();
        characterControl.SetGravity();
        characterControl.SetRotation();
        characterControl.ApplyGroundForce();
        characterControl.ApplyGroundRotation();
    }

    public override void Exit()
    {
        anim.SetFloat("PlaneVelocity", 0f);
    }
    #endregion

    /******************************************/
    /********** 업데이트 메서드 ************/
    /******************************************/

    #region Excute
    public void InputKey()
    {
        characterControl.XMove = Input.GetAxisRaw("Horizontal");
        characterControl.ZMove = Input.GetAxisRaw("Vertical");

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && characterControl.IsOnGround && !characterControl.IsOnMaxAngleSlope)
        {
            characterControl.AddforceForJump();
            characterControl.ChangeState(STATES.JUMP);
            return;
        }

        // 대쉬 입력
        if (Input.GetMouseButtonDown(1) && characterControl.IsOnGround &&
            !characterControl.IsOnMaxAngleSlope && SharedMgr.GameCtrlMgr.GetPlayerCtrl.CanDash())
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.DoDash();
            characterControl.ChangeState(STATES.DASH);
            return;
        }

        // 공격 입력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            characterControl.ChangeState(STATES.ATTACK);
            return;
        }

        // 스킬 입력
        if (Input.GetKeyDown(KeyCode.E))
        {
            characterControl.ChangeState(STATES.SKILL);
            return;
        }

        // 궁극기 입력
        if (Input.GetKeyDown(KeyCode.R))
        {
            characterControl.ChangeState(STATES.ULTIMATESKILL);
            return;
        }
    }


    public void SetPlaneVelocityAnimation()
    {
        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        float magnitude = planeVelocity.magnitude;
        anim.SetFloat("PlaneVelocity", Mathf.Clamp(magnitude, 0f, 5f));
    }

    public void CheckTransitionFallState()
    {
        if(characterControl.IsOnGround == false)
        {
            if (rigid.velocity.y < -0.1f)
                characterControl.ChangeState(STATES.FALL);
            else
                characterControl.ChangeState(STATES.JUMP);
        }
    }
    #endregion
}
