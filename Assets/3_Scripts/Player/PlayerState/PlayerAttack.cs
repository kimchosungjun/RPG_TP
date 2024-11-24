using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : State
{
    PlayerAttackCombo combo = null;

    public PlayerAttack(CharacterCtrl _controller, PlayerAttackCombo _combo) : base(_controller)
    {
        combo = _combo;
    }

    public override void Enter()
    {
        
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        
    }

    public override void FixedExecute()
    {
        throw new System.NotImplementedException();
    }
}
