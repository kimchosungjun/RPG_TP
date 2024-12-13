using UnityEngine;

public class NearAttackShockwaveAction : NearAttackAction
{
    [SerializeField] protected GameObject shockWave;
    public override void StopAttack()
    {
        base.StopAttack();
        Instantiate(shockWave); 
        // Make Shock Wave
    }
}
