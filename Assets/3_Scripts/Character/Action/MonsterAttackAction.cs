using System.Collections;
using UnityEngine;

public abstract class MonsterAttackAction : MonoBehaviour
{
    protected MonsterStat stat = null;
    protected MonsterAttackActionData attackActionData = new MonsterAttackActionData();
    protected MonsterConditionActionData conditionActionData = new MonsterConditionActionData();
    protected bool isCoolDown = true;
    
    public bool GetCoolDown { get { return isCoolDown; } }

    // Set Monster Stat & Attack Action Data
    public virtual void SetData(MonsterStat _stat) { this.stat = _stat; }
    public abstract void DoAttack();
    public virtual void StopAttack() { }

    protected virtual IEnumerator CStartCoolDown(float _coolTime)
    {
        isCoolDown = false;
        yield return new WaitForSeconds(_coolTime);
        isCoolDown = true;
    }

    protected void OnDisable() { isCoolDown = true; }
}
