using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnnoyBear : MonoBehaviour
{
    Vector3 originalPosition;
    int playerLayer = 1 << (int)E_LAYERS.PLAYER;
    [SerializeField] Animator anim;
    enum AnnoyBearState
    {
        Idle = 0,
        Sleep = 1,
        Walk = 2,
        Run = 3,
        Attack = 4,
        Hit = 5,
        Groggy = 6,
        Roar = 7,
        Stamp = 8,
        Death = 9
    }

    [SerializeField] SubBossStatusUICtrl ctrl;
    [SerializeField] NavMeshAgent nav;

    Sequence bearBehaviourState;
    bool isKnockBackState = false; // 넉백을 당할만한 공격에 당한 상태
    bool isGroggyState = false; // 그로기 상태 : 흰 그로기 게이지 감소 시 발동
    public bool IsInBattleField { get { return isInBattleField; } set { isInBattleField = value; } } 
    [SerializeField] bool isInBattleField;
    /// <summary>
    /// 1번행동 : 맞은 상태(넉백, 그냥 맞은 상태) 혹은 그로기 상태인지 확인 => 그냥 맞은 상태라면 2번 행동으로, 그 외에는 그로기 hit 상태
    /// 2번행동 : 전투필드에 있는지 확인 => 없다면 평상시 행동, 있다면 3번 행동으로 
    /// 3번행동 : 인식거리 내에 있는지 확인 => 인식거리 내라면 전투태세, 아니라면 평상시 행동
    /// 4번행동 : 인식거리 내라면 공격 패턴들이 전부 쿨타임인지 확인 후, 쿨타임이라면 주변을 어슬렁 거리면서 공격시간 기다림 
    /// 5번행동 : 쿨타임이 아니라면 다가간다.
    /// 5-1 행동 : 충분히 가까운 거리라면 6번 행동으로 넘어간다.
    /// 6번행동 : 플레이어에게 공격한다.
    /// 6-1번 행동 : 1,2,3번 연속공격 () => 쿨타임이면 4-2로
    /// 6-2 행동 : 내려찍기 공격 (주변에 파동) => 쿨타임이면 4-3로
    /// </summary>

    protected void Start()
    {
        originalPosition = transform.position;
        SetBehaviourState();
        ctrl.Setup(this.transform);
    }

    void SetBehaviourState()
    {
        // Level 3

        // Level 2 
        ActionNode checkbattleField = new ActionNode(DoCheckInBattleField);
        ActionNode doIdle = new ActionNode(DoIdle);
        List<Node> battleFieldList = new List<Node>();
        battleFieldList.Add(checkbattleField);
        battleFieldList.Add(doIdle);

        ActionNode checkInSight = new ActionNode(DoCheckInSight);
        ActionNode doGuardIdle = new ActionNode(DoCheckAttackCool);
        List<Node> sightList = new List<Node>();
        sightList.Add(checkInSight);
        sightList.Add(doGuardIdle);

        // Level 1
        ActionNode checkCanMovable = new ActionNode(DoCheckCanMovable);
        Selector checkBattleFieldGroup = new Selector(battleFieldList);
        Selector checkInSightGroup = new Selector(sightList);
        List<Node> behaviourList = new List<Node>();
        behaviourList.Add(checkCanMovable);
        behaviourList.Add(checkBattleFieldGroup);
        behaviourList.Add(checkInSightGroup);
        // 행동트리 뿌리
        bearBehaviourState = new Sequence(behaviourList);
    }

    private void FixedUpdate()
    {
        ctrl.Execute();
        bearBehaviourState.Evaluate();
    }

    #region 1
    public E_BTS DoCheckCanMovable()
    {
        if (isKnockBackState || isGroggyState) return E_BTS.FAIL;
        else return E_BTS.SUCCESS;
    }

    public void SetHitState()
    {
        // 좀 더 고민해보자
        isKnockBackState = true;
        isGroggyState = true;
    }

    public void ResetHitState()
    {
        isKnockBackState = false;
        isGroggyState = false;
    }
    #endregion

    #region 2
    [SerializeField] float currentIdleTime = 10f;
    [SerializeField] float idleTimer = 5f;
    // selector
    public E_BTS DoCheckInBattleField()
    {
        return (IsInBattleField) ? E_BTS.SUCCESS : E_BTS.FAIL;
    }
    // fail만 반환
    public E_BTS DoIdle()
    {

        if (currentIdleTime <= idleTimer)
        {
            currentIdleTime += Time.deltaTime;
            return E_BTS.FAIL;
        }

        currentIdleTime = 0f;
        int randomBehaviour = Random.Range(0, 3);
        switch (randomBehaviour)
        {
            case (int)AnnoyBearState.Idle:
                anim.SetInteger("State", (int)AnnoyBearState.Idle);
                break;
            case (int)AnnoyBearState.Sleep:
                anim.SetInteger("State", (int)AnnoyBearState.Sleep);
                break;
            case (int)AnnoyBearState.Walk:
                anim.SetInteger("State", (int)AnnoyBearState.Walk);
                break;
        }

        return E_BTS.FAIL;
    }
    #endregion

    #region 3
    public E_BTS DoCheckInSight()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 10f, playerLayer);
        if (colls.Length == 0)
        {
            Debug.Log("인식거리 안이 아닙니다!");
            return E_BTS.FAIL;
        }
        Debug.Log("인식거리 안입니다!");
        return E_BTS.SUCCESS;
    }

    public E_BTS DoGuardIdle()
    {
        currentIdleTime += Time.deltaTime;
        return DoIdle();
    }
    #endregion

    #region 4
    public E_BTS DoCheckAttackCool()
    {
        anim.SetInteger("State", (int)AnnoyBearState.Attack);
        return E_BTS.SUCCESS;
    }
    #endregion
}
