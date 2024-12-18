using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterEnums;

public class Rayfish : CowardMonster
{
    protected List<PathNode> pathNodeGroup = new List<PathNode>();
    public PathNode GetFarPath()
    {
        PathNode bestNode = null;
        float bestDistance = float.MaxValue;
        float maxDistanceSqr = maxDistance * maxDistance;
        foreach (var node in pathNodeGroup)
        {
            float distanceToAI = (node.NodePosition - transform.position).sqrMagnitude;
            if (distanceToAI > maxDistanceSqr) continue;
            float distanceToPlayer = (node.NodePosition - transform.position).sqrMagnitude;
            if (distanceToPlayer > bestDistance)
            {
                bestNode = node;
                bestDistance = distanceToPlayer;
            }
        }
        return bestNode;
    }

    protected override void Awake()
    {
        base.Awake();
        SetPathNodes(Vector3.zero);
    }

    protected override void Update()
    {
        base.Update();
        selectorNode.Evaluate();
    }

    public void SetPathNodes(Vector3 _centerPosition, int _nodeCnt = 10, float _pathRadius = 10f)
    {
        for (int i = 0; i < _nodeCnt; i++)
        {
            float angle = i * Mathf.PI * 2 / _nodeCnt;
            float x = _centerPosition.x + Mathf.Cos(angle) * _pathRadius;
            float z = _centerPosition.z + Mathf.Sin(angle) * _pathRadius;
            Vector3 nodePosition = new Vector3(x, 0, z); 
            pathNodeGroup.Add(new PathNode(nodePosition)); 
        }
    }

    Selector selectorNode;

    protected override void Start()
    {
        Collider coll = GetComponent<Collider>();
        //statusUIController.Setup(this.transform);

        // 레벨 3
        ActionNode isHitByPlayer = new ActionNode(DoIsHitByPlayer);
        ActionNode hitMove = new ActionNode(DoHitRunAway);
        List<Node> btHitLevel3 = new List<Node>();
        btHitLevel3.Add(isHitByPlayer);
        btHitLevel3.Add(hitMove);

        ActionNode isDetect = new ActionNode(DoIsDetectPlayer);
        ActionNode idleMove = new ActionNode(DoIdleBehaviour);
        List<Node> btDetectLevel3 = new List<Node>();
        btDetectLevel3.Add(isDetect);
        btDetectLevel3.Add(idleMove);

        // 레벨 2
        Sequence hitBT = new Sequence(btHitLevel3);
        Selector detectBT = new Selector(btDetectLevel3);
        ActionNode moveBt = new ActionNode(TestMove);
        List<Node> btBehaviourLevel2 = new List<Node>();
        btBehaviourLevel2.Add(hitBT);
        btBehaviourLevel2.Add(detectBT);
        btBehaviourLevel2.Add(moveBt);

        // 레벨 1 : 루트노드

        selectorNode = new Selector(btBehaviourLevel2);
    }

    #region Hit
    /******************************************/
    /************ 애니메이션  ***************/
    /******************************************/
    //public override void TakeDamage()
    //{
    //    isHitState = true;
    //    anim.SetBool("IsEndHit", false);
    //    anim.SetTrigger("Hit");
    //}

    public void EndHitState()
    {
        isHitState = false;
        anim.SetBool("IsEndHit", true);
    }

    /******************************************/
    /****************  행동  ******************/
    /******************************************/
    public NODESTATES DoIsHitByPlayer() { return (isHitState) ? NODESTATES.SUCCESS : NODESTATES.FAIL; }

    public NODESTATES DoHitRunAway()
    {
        Debug.Log("맞아서 도망중");
        if (isMoving) return NODESTATES.SUCCESS;
        StopCoroutine(CDoRunAway());
        StartCoroutine(CDoRunAway());
        return NODESTATES.SUCCESS;
    }

    IEnumerator CDoRunAway()
    {
        PathNode targetNode = GetFarPath();
        // 노드가 없다면 실행 불가
        if (targetNode == null) yield break;

        Vector3 targetPosition = targetNode.NodePosition;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // 현재 위치와 별 차이 없으면 그대로
        if (distance < 0.5f) yield break;

        isMoving = true;
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);
        float movementPercent = 0f;
        while (distance > 0.5f)
        {
            movementPercent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Slerp(currentPosition, targetPosition, movementPercent);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, movementPercent);
            distance = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }
        transform.rotation = targetRotation;
        isMoving = false;
    }
    #endregion

    #region Detect

    public NODESTATES DoIsDetectPlayer()
    {

        float distance = Vector3.Distance(player.position, transform.position);
        if (2f > distance)
        {
            return NODESTATES.SUCCESS;
        }
        else
        {
            return NODESTATES.FAIL;
        }
    }



    public NODESTATES DoIdleBehaviour()
    {
        int randomNodeIndex = 0;
        int randomNum = 0;
        randomNum = Random.Range(0, 2);
        randomNodeIndex = Random.Range(0, pathNodeGroup.Count);
        switch (randomNum)
        {
            // 유휴상태
            case 0:
                if (isIdleMove == false) anim.SetBool("IsMove", false);
                break;
            // 움직임 상태
            case 1:
                if (isIdleMove == false)
                {
                    isIdleMove = true;
                    anim.SetBool("IsMove", true);
                    DoMove(pathNodeGroup[randomNodeIndex].NodePosition);
                }
                break;
        }
        return NODESTATES.SUCCESS;
    }

    public void DoMove(Vector3 _targetPosition) { StartCoroutine(CDoMove(_targetPosition)); }

    IEnumerator CDoMove(Vector3 _targetPosition)
    {
        Vector3 currentPosition = transform.position;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, _targetPosition, time);
            yield return null;
        }
        isIdleMove = false;
    }

    #endregion

    public NODESTATES TestMove()
    {
        Debug.Log("이동중!!");
        return NODESTATES.SUCCESS;
    }
}
