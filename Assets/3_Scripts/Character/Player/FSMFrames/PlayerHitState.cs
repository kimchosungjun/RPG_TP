using UnityEngine;
using EffectEnums;

public class PlayerHitState : PlayerState
{
    float enterTime = 0f;
    public PlayerHitState(PlayerMovementControl _controller) : base(_controller) { }

    public override void Enter()
    {
        characterControl.DoEscapeAttackState();
        characterControl.GetRigid.useGravity = true;
        characterControl.CanChangePlayer = false;
        enterTime = Time.time;
        anim.applyRootMotion = true;
        anim.SetInteger("States", (int)PlayerEnums.STATES.HIT);
        anim.SetBool("IsFallDownGround", false);   
    }


    public override void Execute()
    {

        switch (characterControl.HitCombo)
        {
            case HIT_EFFECTS.FALLDOWN:
                characterControl.GroundCheck();
                if (anim.GetBool("IsFallDownGround") == false && characterControl.IsOnGround)
                    anim.SetBool("IsFallDownGround", true);
                break;
            case HIT_EFFECTS.STUN:
                if(Time.time - enterTime >= characterControl.HitEffectTime)
                    characterControl.ChangeState(PlayerEnums.STATES.MOVEMENT);
                break;
            case HIT_EFFECTS.KNOCKBACK:
                if (Time.time - enterTime >= 0.75f) // Fixe Time
                    characterControl.ChangeState(PlayerEnums.STATES.MOVEMENT);
                    break;
        }
    }

    public override void Exit()
    {
        characterControl.GetRigid.useGravity = false;
        characterControl.CanChangePlayer = true;
        anim.applyRootMotion = false;
        anim.SetBool("IsFallDownGround", false);
    }
}
