using UnityEngine;

public class WarriorNormalAttack : NearAttackAction
{
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] Collider attackCollider;
    [SerializeField] PlayerNormalAttackActionSOData soData;    

    bool isAttackState;

    #region SetValue
    public void SetupData()
    {
        isAttackState = false;
     
    }
    #endregion

    public override void DoAction()
    {
        isAttackState = true;
        base.DoAction(); // Do Attack
    }
    
    public override void StopAttack()
    {
        isAttackState = false;
        base.StopAttack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttackState && other.gameObject.layer == enemyLayerMask)
        {
            if (CheckCollider(other))
            {
                // To Do ~~~~~~~
                // 스탯에 맞는 공격력을 반환하여 적에게 데미지를 입힌다.
            }
        }
    }
}
