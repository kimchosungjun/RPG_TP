using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerOnGroundState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Creator & Value

    int currentCombo = -1;
    protected Animator anim = null;
    protected PlayerAttackCombo attackCombo = null;
    public PlayerAttackState(CharacterMovement _controller, PlayerAttackCombo _attackCombo) : base(_controller)
    {
        this.anim = _controller.Anim;
        this.attackCombo = _attackCombo;
    }

    #endregion

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame 

    public override void Enter()
    {
        currentCombo = attackCombo.GetCombo();
        anim.SetBool("IsAttackEnd", false);
        //anim.SetFloat("PlaneVelocity", 0f);
        anim.SetInteger("AttackCombo", currentCombo);
        anim.SetTrigger("Attack"); 
    }

    public override void Execute()
    {
        if (characterCtrl.CanPlayerCtrl == false) return;
        base.Execute();
    }

    public bool CanExecute() { return true; }

    public override void Exit() { attackCombo.SetComboTime();  }

    #endregion
}
