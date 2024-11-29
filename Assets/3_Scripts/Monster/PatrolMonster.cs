using System.Collections;
using UnityEngine;

public class PatrolMonster : NonCombatMonster
{
    // 맞은 상태에서는 움직이지 말아야한다. 
    // Hit 애니메이션 도중 이동하면 문워크하는 느낌이 나기 때문
    [SerializeField] protected bool isHitState = false;
    [SerializeField] protected Vector3[] pathWays;
    [SerializeField] protected float moveSpeed;
    protected int currentWayPoint = 0;
    protected int maxWayPoint = 0;
    protected Vector3 targetWay;
    protected Vector3 lastWay;

    protected virtual void Awake()
    {
        maxWayPoint = pathWays.Length;
        targetWay = pathWays[0];
    }

    /// <summary>
    /// Success값만 반환
    /// </summary>
    /// <returns></returns>
    protected E_BT DoPatrol()
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

        return E_BT.BT_SUCCESS;
    }
}
