using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flybee : PatrolMonster
{
    // 맞은 상태에서는 움직이지 말아야한다. 
    // Hit 애니메이션 도중 이동하면 문워크하는 느낌이 나기 때문
    [SerializeField] protected bool isHitState = false;

    // 하나라도 참이면 바로 참을 반환 
    Selector flybeeRoot = null;

    protected void Start()
    {
        // Level1 
        ActionNode checkCurrentStateAction = new ActionNode(DoCheckHitState);
        ActionNode patrolAction = new ActionNode(DoPatrol);
        List<Node> level1 = new List<Node>();

        // Link Level1 : 피격상태 확인 후 패트롤
        level1.Add(checkCurrentStateAction);    
        level1.Add(patrolAction);

        // Root
        flybeeRoot = new Selector(level1);
    }

    protected void Update()
    {
        flybeeRoot.Evaluate();
        if (Input.GetKeyDown(KeyCode.B)) MakeHitState();
    }

    public E_BT DoCheckHitState()
    {
        if (isHitState) return E_BT.BT_SUCCESS;
        else return E_BT.BT_FAIL;   
    }

    protected override E_BT DoPatrol()
    {
        if (Vector3.Distance(transform.position, targetWay) < 0.2f)
        {
            currentWayPoint += 1;
            if (currentWayPoint >= maxWayPoint)
                currentWayPoint = 0;
            targetWay = pathWays[currentWayPoint];
        }
        // 회전 : 나중에 수정
        transform.rotation = Quaternion.LookRotation(targetWay - transform.position);

        Vector3 moveDirection = (targetWay - transform.position).normalized;
        transform.position += moveDirection * Time.deltaTime * moveSpeed;

        return E_BT.BT_SUCCESS;
    }

    #region 테스트용 메서드
    public void MakeHitState()
    {
        isHitState= true;
        anim.SetBool("IsHit", true);
    }

    public void ReturnHitState()
    {
        isHitState = false;
        anim.SetBool("IsHit", false);
    }
    #endregion
}
