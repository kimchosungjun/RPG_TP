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
            redDragon.ChasePlayer();
            //if (redDragon.IsInAttackRange())
            //{

            //}
            //else
            //{
            //}
        }
    }

    #endregion
}

