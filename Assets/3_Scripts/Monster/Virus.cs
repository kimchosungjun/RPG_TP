using MonsterEnums;
using System.Collections.Generic;
using UnityEngine;

public class Virus : CombatMonster
{
    // 맞은 상태에서는 움직이지 말아야한다. 
    // Hit 애니메이션 도중 이동하면 문워크하는 느낌이 나기 때문
    [SerializeField] protected bool isHitState = false;
    [SerializeField] protected bool isDeathState = false;
    [SerializeField] Rigidbody rigid;
    // 하나라도 참이면 바로 참을 반환 
    Selector flybeeRoot = null;

    protected override void Start()
    {
        base.Start();
        if (anim == null) anim = GetComponent<Animator>();
        if (statusUI == null) statusUI = GetComponentInChildren<StandardMonsterStatusUI>();
    }

    protected override void CreateBTStates()
    {
        // Level1 
        ActionNode checkCurrentStateAction = new ActionNode(DoCheckHitState);
        //ActionNode patrolAction = new ActionNode(DoPatrol);
        List<Node> level1 = new List<Node>();

        // Link Level1 : 피격상태 확인 후 패트롤
        level1.Add(checkCurrentStateAction);
        //level1.Add(patrolAction);

        // Root
        flybeeRoot = new Selector(level1);
    }

    protected override void FixedUpdate()
    {
        if (isDeathState) return;
        flybeeRoot.Evaluate();
        statusUI.FixedExecute();
    }

    public BTS DoCheckHitState()
    {
        if (isHitState) return BTS.SUCCESS;
        else return BTS.FAIL;
    }

    public override void ApplyMovementTakeDamage(TransferAttackData _attackData)
    {
        switch (_attackData.GetHitEffect)
        {
            case EffectEnums.HIT_EFFECTS.STUN:
                break;
            default:
                isHitState = true;
                anim.SetInteger("MState", (int)STATES.HIT);
                break;
        }
    }

    public override void Death()
    {
        base.Death();
        rigid.isKinematic = false;
        statusUI.gameObject.SetActive(false);
        isDeathState = true;
        rigid.useGravity = true;
    }

    public void EscapeHitState() { isHitState = false; anim.SetInteger("MState", (int)STATES.MOVE); }
}
