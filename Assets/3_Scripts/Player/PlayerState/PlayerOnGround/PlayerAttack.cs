using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerOnGroundState
{
    int currentCombo = -1;
    PlayerAttackCombo attackCombo = null;
    public PlayerAttack(CharacterCtrl _controller, PlayerAttackCombo _attackCombo) : base(_controller)
    {
        attackCombo = _attackCombo;
    }

    public override void Enter()
    {
        currentCombo = attackCombo.GetCombo();
        characterCtrl.Anim.SetInteger("AttackCombo", currentCombo);
        characterCtrl.Anim.SetTrigger("Attack"); 
    }
    public override void Execute() {   }
    public override void FixedExecute() {  }
    public override void Exit() {  }
}
