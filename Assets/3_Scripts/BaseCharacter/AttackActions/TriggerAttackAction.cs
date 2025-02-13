using System.Collections.Generic;
using UnityEngine;

public class TriggerAttackAction : AttackAction
{
    [SerializeField] protected HitTrigger triggerAttack = null;
    public override void DoAttack()
    {
        if (triggerAttack != null)
        {
            triggerAttack.SetHitData(attackData , conditionData);
            triggerAttack.gameObject.SetActive(true);
        }
    }

    public override void StopAttack()
    {
        if(triggerAttack!=null)
            triggerAttack.gameObject.SetActive(false);
    }
}
