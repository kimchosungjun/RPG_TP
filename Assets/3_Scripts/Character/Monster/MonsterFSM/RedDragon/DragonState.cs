using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragondStateClasses
{
    #region Dragon Base State Class
    public class DragonState : State
    {
        protected RedDragon redDragon = null;
        public DragonState(RedDragon _redDragon)
        {
            redDragon = _redDragon;
        }

        public override void Enter() { }

        public override void Exit() { }
    }
    #endregion


    #region Idle
    public class DragonIdleState : DragonState
    {
        public DragonIdleState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.IDLE);
        }

        public override void FixedExecute()
        {
            redDragon.CheckPlayerInArea();
        }
    }
    #endregion

    #region Move
    public class DragonMoveState : DragonState
    {
        public DragonMoveState(RedDragon _redDragon) : base(_redDragon) { }
        float moveSpeed = 3f;

        public override void Enter()
        {
            redDragon.GetNav.speed = moveSpeed;
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.MOVE);
        }

        public override void FixedExecute()
        {
            if (redDragon.IsInAttackRange())
                redDragon.ChangeState(RedDragon.DRAGON_STATE.BATTLE);
            else
                redDragon.ChasePlayer();
        }
    }

    #endregion

    #region Attack
    public class DragonAttackState : DragonState
    {
        public DragonAttackState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            redDragon.GetNav.ResetPath();
            int randomAttack = Randoms.GetRandomCnt(0, 3);
            redDragon.ChargeAttackEnergy(0.5f);
            redDragon.SetAttackAnimation(RedDragon.DRAGON_ANIM.ATTACK, randomAttack);
        }
    }
    #endregion

    #region Takeoff , Glide, Land
    public class DragonTakeOffState : DragonState
    {
        public DragonTakeOffState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.TAKEOFF);
        }
    }

    public class DragonGlideState : DragonState
    {
        public DragonGlideState(RedDragon _redDragon) : base(_redDragon) { }

        float glideSpeed = 5f;
        float stopDist = 0.2f;
        float defaultDist= 0f;
        public override void Enter()
        {
            defaultDist = redDragon.GetNav.stoppingDistance;
            redDragon.GetNav.stoppingDistance = stopDist;
            redDragon.GetNav.speed = glideSpeed;
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.GLIDE);
            redDragon.GlideAttack();
        }

        public override void Exit()
        {
            redDragon.GetNav.stoppingDistance = defaultDist;
        }
    }

    public class DragonLandState : DragonState
    {
        public DragonLandState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.LAND);
        }

        public override void Exit()
        {
            redDragon.IsInvincible = false;
        }
    }
    #endregion

    #region Hit
    public class DragonHitState : DragonState
    {
        public DragonHitState(RedDragon _redDragon) : base(_redDragon) { }

        EffectEnums.HIT_EFFECTS effect;
        bool checkMaintainTime = false;
        float maintainTime = 0f;
        float time = 0f;

        public void SetEffect(TransferAttackData _attackData)
        {
            effect = _attackData.GetHitEffect;
            if(_attackData.GetHitEffect == EffectEnums.HIT_EFFECTS.STUN)
            {
                checkMaintainTime = true;
                maintainTime = _attackData.EffectMaintainTime;
            }
        }

        public void SetEffect(EffectEnums.HIT_EFFECTS _effect, float _time)
        {
            effect = _effect;
            maintainTime = _time;
            if (_effect == EffectEnums.HIT_EFFECTS.STUN)
                checkMaintainTime = true;
        }

        public override void Enter()
        {
            time = 0f;
            if(effect == EffectEnums.HIT_EFFECTS.STUN)
                redDragon.SetAnimation(RedDragon.DRAGON_ANIM.GROGGY);
            else
                redDragon.SetAnimation(RedDragon.DRAGON_ANIM.HIT);
        }

        public override void FixedExecute()
        {
            if (checkMaintainTime)
            {
                time += Time.fixedDeltaTime;
                if (time >= maintainTime)
                {
                    redDragon.ChangeState(RedDragon.DRAGON_STATE.MOVE);
                    return;
                }
            }
        }
    }
    #endregion

    #region Death

    public class DragonDeathState : DragonState
    {
        public DragonDeathState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            // Get Item & Exp
            // Change Layer
            
        }

        public override void Exit()
        {
            // Active GameObject
            // Reset Stat
        }
    }

    #endregion
}

