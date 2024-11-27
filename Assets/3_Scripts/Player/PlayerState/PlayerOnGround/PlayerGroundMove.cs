using System.Collections;
using UnityEngine;

public class PlayerGroundMove : PlayerOnGroundState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Createor & Value
    Rigidbody rigid = null;
    Animator anim = null;
    public PlayerGroundMove(CharacterCtrl _controller) : base(_controller) 
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
        InputKey();
        SetPlaneVelocityAnimation();
        CheckTransitionFallState();
    }

    public override void FixedExecute()
    {
        characterCtrl.SetMoveDirection();
        characterCtrl.SetSlopeMoveDirection();
        characterCtrl.SetGravity();
        characterCtrl.SetRotation();
        characterCtrl.ApplyMoveForce();
        characterCtrl.ApplyMoveRotation();
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
            Jumping();

        // 대쉬 입력
        if (Input.GetKeyDown(KeyCode.Q) && characterCtrl.IsOnGround && !characterCtrl.IsOnMaxAngleSlope)
            characterCtrl.ChangeState(E_PLAYER_FSM.DASH);
    }

    public void Jumping()
    {
        //rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        //rigid.AddForce(Vector3.up * characterCtrl.PlayerJumpForce, ForceMode.Impulse);
        characterCtrl.ChangeState(E_PLAYER_FSM.JUMP);
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
                characterCtrl.ChangeState(E_PLAYER_FSM.FALL);
        }
    }
    #endregion
}
