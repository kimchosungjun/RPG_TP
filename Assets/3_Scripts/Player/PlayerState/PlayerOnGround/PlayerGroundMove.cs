using System.Collections;
using UnityEngine;

public class PlayerGroundMove : PlayerState
{
    Rigidbody rigid = null;
    Animator anim = null;
    public PlayerGroundMove(CharacterCtrl _controller) : base(_controller) 
    {
        this.rigid = _controller.Rigid;
        this.anim = _controller.Anim;
    }

    #region StateMachine Frame
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        characterCtrl.LimitMovementSpeed();
        characterCtrl.GroundCheck();
        InputKey();
        SetAnimation();
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

    public override void Exit()
    {
        
    }
    #endregion

    #region Excute
    public void InputKey()
    {
        characterCtrl.XMove = Input.GetAxisRaw("Horizontal");
        characterCtrl.ZMove = Input.GetAxisRaw("Vertical");

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && characterCtrl.IsOnGround && !characterCtrl.IsOnMaxAngleSlope)
            Jumping();

        // 대쉬 입력
        if (Input.GetKeyDown(KeyCode.Q) && characterCtrl.IsOnGround)
            Dashing();
    }

    public void Jumping()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * characterCtrl.PlayerJumpForce, ForceMode.Impulse);
        characterCtrl.ChangeState(E_PLAYER_FSM.JUMP);
    }

    public void Dashing()
    {
        characterCtrl.ChangeState(E_PLAYER_FSM.DASH);
        characterCtrl.Dash();
    }

    public void SetAnimation()
    {
        if (characterCtrl.IsOnMaxAngleSlope)
        {
            anim.SetFloat("PlaneVelocity", 0f);
            return;
        }
        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        float magnitude = planeVelocity.magnitude;
        anim.SetFloat("PlaneVelocity", Mathf.Clamp(magnitude, 0f, 5f));
    }
    #endregion

    #region FixedExecute

    #endregion
}
