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

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.MOVE);
        }

        public override void FixedExecute()
        {
            if (redDragon.IsInAttackRange())
            {
                redDragon.DoAttack();
                redDragon.ChangeState(RedDragon.DRAGON_STATE.TAKEOFF);
            }
            else
                redDragon.ChasePlayer();
        }
    }

    #endregion

    #region Attack
    public class DragonAttackState : DragonState
    {
        float defaultSpeed = 0;
        public DragonAttackState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            defaultSpeed = redDragon.GetNav.speed;
            redDragon.GetNav.speed = 0;
            int randomAttack = Randoms.GetRandomCnt(0, 3);
            redDragon.ChargeAttackEnergy(0.5f);
            redDragon.SetAttackAnimation(RedDragon.DRAGON_ANIM.ATTACK, randomAttack);
        }

        public override void Exit()
        {
            redDragon.GetNav.speed = defaultSpeed;
        }
    }
    #endregion

    #region 
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

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.GLIDE);
            redDragon.GlideAttack();
        }
    }

    public class DragonLandState : DragonState
    {
        public DragonLandState(RedDragon _redDragon) : base(_redDragon) { }

        public override void Enter()
        {
            redDragon.SetAnimation(RedDragon.DRAGON_ANIM.LAND);
        }
    }
    #endregion
}

