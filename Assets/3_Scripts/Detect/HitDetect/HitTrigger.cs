using UnityEngine;

public class HitTrigger : HitBox
{
    [SerializeField, Tooltip("다중 공격 여부")] protected bool isMultiAttack;
    public override void Active()
    {
        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);
    }

    public override void InActive()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)enemyLayer)
        {
            BaseActor baseActor =other.GetComponentInParent<BaseActor>();
            baseActor?.TakeDamage(attackData);
            if (isMultiAttack == false) InActive();
            if (enemyLayer == UtilEnums.LAYERS.MONSTER)
            {
                Transform hitParticle = SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.MON_DAMAGED);
                RaycastHit hit;
                Vector3 direction = (other.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, direction, out hit))
                {
                    hitParticle.transform.position = hit.point;
                    hitParticle.gameObject.SetActive(true);
                }
            }
        }
    }
}
