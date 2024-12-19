using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : ActorStatControl
{
    MonsterStat monsterStat = null;
    BaseMonster baseMonster = null;
    
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    public void SetBaseMonster(BaseMonster _baseMonster) { this.baseMonster = _baseMonster; }
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

        // 체력을 계산하여 죽은 상태인지 확인
        float curHp = monsterStat.CurrentHP - allDamage;
        if (curHp <= 0)
        {
            monsterStat.CurrentHP = 0f;
            baseMonster.Death();
        }
        else
        {
            monsterStat.CurrentHP = curHp;
        }

        // 데미지를 스탯에 적용 후 UI에 표기
        base.TakeDamage(_attackData);
    }

    /// <summary>
    /// 다시 태어날 때 리셋
    /// </summary>
    public void ResetStat()
    {

    }
}
