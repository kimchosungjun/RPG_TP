using System.Collections;
using UnityEngine;
using MonsterEnums;
using DragondStateClasses;

public enum DRAGON_ATTACK
{
    BASIC = 0,
    CLAW_1=1,
    CLAW_2=2,
    FLAME=3,
    FLAEM_ORBIT=4,
}

public class RedDragonAttackControl : MonoBehaviour
{
    MonsterStat stat = null;
    MonsterAttack[] attackDatas; // Basic, Claw_1, Claw_2, Flame, Orbit Falme
    [SerializeField] TriggerAttackAction[] attacks;
    protected bool isCoolDown = true;

    DRAGON_ATTACK lastAttack;
    public bool GetCoolDown { get { return isCoolDown; } }

    public void SetData(MonsterStat _stat)
    {
        stat = _stat;
        attackDatas = new MonsterAttack[5];
        attackDatas[0] = new MonsterAttack(_stat, ATTACK_ACTIONS.DRAGON_BASIC);
        attackDatas[1] = new MonsterAttack(_stat, ATTACK_ACTIONS.DRAGOND_CLAW_1);
        attackDatas[2] = new MonsterAttack(_stat, ATTACK_ACTIONS.DRAGOND_CLAW_2);
        attackDatas[3] = new MonsterAttack(_stat, ATTACK_ACTIONS.DRAGON_FLAME);
        attackDatas[4] = new MonsterAttack(_stat, ATTACK_ACTIONS.DRAGOND_ORBIT_FLAME);

        SetAttackData(DRAGON_ATTACK.BASIC);
        SetAttackData(DRAGON_ATTACK.CLAW_1);
        SetAttackData(DRAGON_ATTACK.CLAW_2);
        SetAttackData(DRAGON_ATTACK.FLAME);
        //SetAttackData(DRAGON_ATTACK.FLAEM_ORBIT);
    }

    public void SetAttackData(DRAGON_ATTACK _attack)
    {
        int index = (int)_attack;
        // Attack Data
        TransferAttackData attackData = null;
        if (attackDatas[index].GetAttackData!= null)
        {
            attackData = new TransferAttackData();
            attackData.SetData(attackDatas[index].GetAttackData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical)
* attackDatas[index].GetAttackData.GetMultiplier, attackDatas[index].GetAttackData.GetMaintainTime);
        }

        // Condition Data
        TransferConditionData conditionData = null;
        if (attackDatas[index].GetConditionData != null)
        {
            conditionData = new TransferConditionData();
            conditionData.SetData(stat, attackDatas[index].GetConditionData.GetEffect, attackDatas[index].GetConditionData.GetAttribute,
       attackDatas[index].GetConditionData.GetConditionType, attackDatas[index].GetConditionData.GetDefaultValue, attackDatas[index].GetConditionData.GetMaintainTime, attackDatas[index].GetConditionData.GetMultiplier);
        }
   
        attacks[index].SetTransferData(attackData, conditionData, false);
    }

    public void DoAttack(DRAGON_ATTACK _attack) { lastAttack = _attack;  attacks[(int)_attack].DoAttack(); }

    public void StopAttack(DRAGON_ATTACK _attack) {attacks[(int)_attack].StopAttack(); }

    public void DoOrbitFlameAttack()
    {
        int orbitIndex = (int)DRAGON_ATTACK.FLAEM_ORBIT;
        // Attack Data
        TransferAttackData attackData = null;
        if (attackDatas[orbitIndex].GetAttackData != null)
        {
            attackData = new TransferAttackData();
            attackData.SetData(attackDatas[orbitIndex].GetAttackData.GetEffect, stat.Attack * Randoms.GetCritical(stat.Critical)
* attackDatas[orbitIndex].GetAttackData.GetMultiplier, attackDatas[orbitIndex].GetAttackData.GetMaintainTime);
        }

        // Condition Data
        TransferConditionData conditionData = null;
        if (attackDatas[orbitIndex].GetConditionData != null)
        {
            conditionData = new TransferConditionData();
            conditionData.SetData(stat, attackDatas[orbitIndex].GetConditionData.GetEffect, attackDatas[orbitIndex].GetConditionData.GetAttribute,
       attackDatas[orbitIndex].GetConditionData.GetConditionType, attackDatas[orbitIndex].GetConditionData.GetDefaultValue, attackDatas[orbitIndex].GetConditionData.GetMaintainTime, attackDatas[orbitIndex].GetConditionData.GetMultiplier);
        }

        Transform orbitTransform = SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.GUIDED_FIRE);
        orbitTransform.transform.position = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        orbitTransform.GetComponent<HitChaseOverlap>().SetHitData(attackData, conditionData);
    }

    public void DoBasicAttack() { DoAttack(DRAGON_ATTACK.BASIC); }
    public void DoFirstClawAttack() { DoAttack(DRAGON_ATTACK.CLAW_1); }
    public void DoSecondClawAttack() { DoAttack(DRAGON_ATTACK.CLAW_2); }
    public void DoFlameAttack() { DoAttack(DRAGON_ATTACK.FLAME); }
}
