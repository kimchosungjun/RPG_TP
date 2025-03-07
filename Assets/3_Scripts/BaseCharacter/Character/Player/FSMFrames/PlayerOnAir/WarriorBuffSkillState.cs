using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorBuffSkillState : PlayerSkillState
{
    WarriorMovementControl warriorMovement;
    public WarriorBuffSkillState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo)
    {
        this.attackCombo = _attackCombo;
        warriorMovement = _controller.GetComponent<WarriorMovementControl>();
    }

    public override void Enter()
    {
        base.Enter();
        //warriorMovement.GetWarriorActionControl.DoBuffSkill();
    }

    public override void Exit() { base.Exit(); }
}
