using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    /******************************************/
    /*********** 생성자 & 변수  *************/
    /******************************************/

    #region Creator & Value

    int currentCombo = -1;
    PlayerStat playerStat = null;
    public PlayerAttackState(PlayerMovementControl _controller, PlayerAttackCombo _attackCombo) : base(_controller, _attackCombo) 
    {
        playerStat = _controller.GetComponent<PlayerStatControl>()?.PlayerStat;
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
        characterControl.transform.rotation = CheckColliderRotate();
        anim.SetInteger("Combo", currentCombo);
        anim.SetFloat("AttackSpeed", playerStat.AttackSpeed);
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


    public Quaternion CheckColliderRotate()
    {
        Transform characterTransform = characterControl.transform;  
        int enemyLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        Collider[] colls = Physics.OverlapSphere(characterTransform.position, attackCombo.GetAttackRange, enemyLayer);
        int collCnt= colls.Length;
        
        if(collCnt==0)
            return characterControl.transform.rotation;

        int nearIndex = 0;
        float nearDistance = Vector3.Distance(characterTransform.position, colls[0].transform.position);
        for(int i=1; i<collCnt; i++)
        {
            float distance = Vector3.Distance(characterTransform.position, colls[i].transform.position);
            if (nearDistance > distance)
            {
                nearDistance = distance;
                nearIndex = i;
            }
        }

        Vector3 direction = colls[nearIndex].transform.position - characterTransform.position;
        direction.y = 0;
        direction = direction.normalized;
        return Quaternion.LookRotation(direction);
    }
    #endregion
}
