using MonsterEnums;
using UnityEngine;

public class NonCombatMonster : NormalMonster
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

    public override BTS DetectPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override BTS IdleMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery()
    {
        throw new System.NotImplementedException();
    }


    #endregion
}
