using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCombatMonster : BaseMonster
{
    //나중에 지울 변수
    [SerializeField] protected float tempHP;
    [SerializeField] protected float tempCurHp;


    /******************************************/
    /****** 비전투 유닛 공통 변수  ********/
    /******************************************/

    #region Variable
    [SerializeField, Header("회복에 걸리는 시간 : 공격받지 않은 시간")] protected float recoveryTimer = 10f;
    protected float recoveryTime = 0f;
    protected bool canRecovery = false;
    #endregion

    /******************************************/
    /*****************  변수  *****************/
    /******************************************/
    #region Recovery 
    public virtual void MeasureRecoveryTime()
    {
        if (canRecovery == false && recoveryTime > recoveryTimer) { canRecovery = true; return; }
        recoveryTime += Time.deltaTime;
    }

    /// <summary>
    /// 맞았을 때 호출
    /// </summary>
    public virtual void ResetRecoveryTime()
    {
        recoveryTime = 0f;
        canRecovery = false;
    }
    #endregion

    #region Override
    public override void Death()
    {
        throw new System.NotImplementedException();
    }

    public override E_BT DetectMovement()
    {
        throw new System.NotImplementedException();
    }

    public override E_BT DetectPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override E_BT IdleMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery()
    {
        throw new System.NotImplementedException();
    }

    public override void Spawn()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
