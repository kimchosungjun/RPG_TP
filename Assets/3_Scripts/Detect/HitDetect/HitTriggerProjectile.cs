using System.Collections;
using UnityEngine;

public class HitTriggerProjectile : HitBox
{
    [SerializeField, Tooltip("투사체가 유지되는 시간")] protected float visibleTime = 10f;
    [SerializeField, Tooltip("날라가는 속도")] protected float speed = 10f;
    [SerializeField, Tooltip("관통 여부")] protected bool isPenetrate;

    int wallLayer = (int)UtilEnums.LAYERS.WALL;
    public override void Active()
    {
        if(this.gameObject.activeSelf==false)
            this.gameObject.SetActive(true);
        StartCoroutine(CToTarget());
    }

    IEnumerator CToTarget()
    {
        float ctime = 0;
        while (ctime <= visibleTime) 
        {
            ctime += Time.deltaTime;
            transform.position += moveDirection* speed * Time.deltaTime;
            yield return null;
        }
        InActive();
    }

    public override void InActive()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)enemyLayer)
        {
            other.GetComponent<BaseActor>()?.TakeDamage(attackData);
            if (isPenetrate == false) InActive();
        }

        if(other.gameObject.layer == wallLayer)
            InActive();
    }
}
