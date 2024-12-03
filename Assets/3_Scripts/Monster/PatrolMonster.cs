using System.Collections;
using UnityEngine;

public class PatrolMonster : NonCombatMonster
{  
    [SerializeField] protected Vector3[] pathWays;
    [SerializeField] protected float moveSpeed;
    protected int currentWayPoint = 0;
    protected int maxWayPoint = 0;
    protected Vector3 targetWay;
    protected Vector3 lastWay;

    protected override void Awake()
    {
        maxWayPoint = pathWays.Length;
        targetWay = pathWays[0];
    }

    /// <summary>
    /// Success값만 반환
    /// </summary>
    /// <returns></returns>
    protected virtual E_BTS DoPatrol()
    {
        if(Vector3.Distance(transform.position, targetWay) < 0.2f)
        {
            currentWayPoint += 1;
            if(currentWayPoint >= maxWayPoint)
                currentWayPoint = 0;
            targetWay = pathWays[currentWayPoint];
        }

        Vector3 moveDirection = (targetWay - transform.position).normalized;
        transform.position += moveDirection * Time.deltaTime * moveSpeed;

        return E_BTS.SUCCESS;
    }
}
