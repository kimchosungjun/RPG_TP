using System.Collections.Generic;
using UnityEngine;

public class NearTriggerAttackAction : AttackAction
{
    [SerializeField] protected HitTriggerAreaBox triggerAttack = null;

    public override void DoAttack()
    {
        if (triggerAttack != null)
        {
            triggerAttack.SetHitData(attackDatas[0],null);
            triggerAttack.gameObject.SetActive(true);
        }
    }

    public override void StopAttack()
    {
        if(triggerAttack!=null)
            triggerAttack.gameObject.SetActive(false);
    }
}
