using UnityEngine;
using EffectEnums;

public class PlayerHitState : PlayerState
{
    float enterTime = 0f;
    public PlayerHitState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        base.Enter();
        enterTime = Time.time;  
        anim.applyRootMotion = true;
        anim.SetInteger("States", (int)PlayerEnums.STATES.HIT);
    }

    public override void Execute()
    {
        base.FixedExecute();

        switch (characterControl.HitCombo)
        {
            case HIT_EFFECTS.FALLDOWN:
                if (characterControl.IsOnGround)
                    characterControl.ChangeState(PlayerEnums.STATES.MOVEMENT);
                break;
            case HIT_EFFECTS.STUN:
                if(Time.time - enterTime >= characterControl.HitEffectTime)
                    characterControl.ChangeState(PlayerEnums.STATES.MOVEMENT);
                break;
            case HIT_EFFECTS.KNOCKBACK:
                if (Time.time - enterTime >= 0.1f) // Fixe Time
                    characterControl.ChangeState(PlayerEnums.STATES.MOVEMENT);
                    break;
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.applyRootMotion = false;   
    }
}
