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
    Animator anim = null;
    public PlayerGroundMoveState(WarriorMovement _controller) : base(_controller) 
    {
        this.rigid = _controller.Rigid;
        this.anim = _controller.Anim;
    }
    #endregion

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame

    public override void Execute()
    {
        characterCtrl.LimitMovementSpeed();
        characterCtrl.GroundCheck();
        SetPlaneVelocityAnimation();
        CheckTransitionFallState();
        InputKey();
    }

    public override void FixedExecute()
    {
        characterCtrl.SetMoveDirection();
        characterCtrl.SetSlopeMoveDirection();
        characterCtrl.SetGravity();
        characterCtrl.SetRotation();
        characterCtrl.ApplyGroundForce();
        characterCtrl.ApplyGroundRotation();
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
        characterCtrl.XMove = Input.GetAxisRaw("Horizontal");
        characterCtrl.ZMove = Input.GetAxisRaw("Vertical");

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && characterCtrl.IsOnGround && !characterCtrl.IsOnMaxAngleSlope)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            rigid.AddForce(Vector3.up * characterCtrl.PlayerJumpForce, ForceMode.Impulse);
            characterCtrl.ChangeState(PlayerEnums.STATES.JUMP);
            return;
        }

        // 대쉬 입력
        if (Input.GetMouseButtonDown(1) && characterCtrl.IsOnGround && !characterCtrl.IsOnMaxAngleSlope)
        {
            characterCtrl.ChangeState(PlayerEnums.STATES.DASH);
            return;
        }

        // 공격 입력
        if (/*Input.GetMouseButtonDown(0)*/ Input.GetKeyDown(KeyCode.Q))
        {
            characterCtrl.ChangeState(PlayerEnums.STATES.ATTACK);
            return;
        }

        // 스킬 입력
        if (Input.GetKeyDown(KeyCode.E))
        {
            characterCtrl.ChangeState(PlayerEnums.STATES.SKILL);
            return;
        }

        // 궁극기 입력
        if (Input.GetKeyDown(KeyCode.R))
        {
            characterCtrl.ChangeState(PlayerEnums.STATES.ULTIMATESKILL);
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
        if(characterCtrl.IsOnGround == false)
        {
            if (rigid.velocity.y < -0.1f)
                characterCtrl.ChangeState(PlayerEnums.STATES.FALL);
            else
                characterCtrl.ChangeState(PlayerEnums.STATES.JUMP);
        }
    }
    #endregion
}
