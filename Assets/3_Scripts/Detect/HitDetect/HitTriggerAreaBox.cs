using UnityEngine;

public class HitTriggerAreaBox : HitBox
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
            other.GetComponent<BaseActor>().TakeDamage(attackData);
            if (isMultiAttack == false) InActive();
        }
    }
}
