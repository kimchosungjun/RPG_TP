using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : ActorStatControl
{
    MonsterStat monsterStat = null;
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    public override void Heal(float _heal)
    {
        base.Heal(_heal);
        throw new System.NotImplementedException();
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        base.Recovery(_percent, _time); 
        // 현재 체력과 최대체력 비교
        //monsterStat.HP = 
    }

    IEnumerator CRecovery(float _percent)
    {
        yield return null;  
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        // 방어력이 공격력보다 높은 경우 데미지 1이라도 들어가도록 
        float allDamage = _attackData.GetAttackValue - monsterStat.Defence;
        allDamage = (allDamage) <= 0 ? 1 : allDamage;
        // fillAmount가 음수가 되는것을 방지
        monsterStat.MonsterCurHP = (monsterStat.MonsterCurHP-allDamage<=0) ? 0f : monsterStat.MonsterCurHP - allDamage;
        base.TakeDamage(_attackData);
    }

    /// <summary>
    /// 다시 태어날 때 리셋
    /// </summary>
    public void ResetStat()
    {

    }
}
