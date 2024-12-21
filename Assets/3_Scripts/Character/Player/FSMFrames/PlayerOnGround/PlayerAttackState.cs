public class PlayerAttackState : PlayerActionState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Creator & Value

    int currentCombo = -1;
    public PlayerAttackState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo) 
    {

    }

    #endregion

    /******************************************/
    /********** 상태머신 재정의  ***********/
    /******************************************/

    #region StateMachine Frame 

    public override void Enter()
    {
        base.Enter();   
        currentCombo = attackCombo.GetCombo();
        anim.SetInteger("Combo", currentCombo);
        anim.SetFloat("AttackSpeed", 1f);
        anim.SetInteger("States", (int)PlayerEnums.STATES.ATTACK);
    }

    public override void Execute()
    {
        base.Execute();
    }

    public bool CanExecute() { return true; }

    public override void Exit() 
    {
        base.Exit();
        attackCombo.SetComboTime();  
    }

    #endregion
}
