using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerJump : PlayerOnAir
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    bool maintainJumpState = true;
    float time = 0f;
    float timer = 0.1f;

    public PlayerJump(CharacterCtrl _controller) : base(_controller) {  }

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame
    public override void Enter()
    {
        Debug.Log("점프");
        base.Enter();
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * characterCtrl.PlayerJumpForce, ForceMode.Impulse);
        characterCtrl.IsOnGround = false;
        anim.SetBool("IsOnGround", false);
        anim.SetTrigger("Jump");
        time = 0f;
        maintainJumpState = true;
    }

    public override void Execute()
    {
        time += Time.deltaTime;
        if (time > timer) maintainJumpState = false;

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
        characterCtrl.ApplyMoveForce();
        characterCtrl.ApplyMoveRotation();
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
            characterCtrl.ChangeState(E_PLAYER_FSM.MOVEMENT);
        }
        else
        {
            if (rigid.velocity.y < 0)
            {
                characterCtrl.ChangeState(E_PLAYER_FSM.FALL);
            }
        }
    }
    #endregion
}
