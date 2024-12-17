using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : CharacterStatControl
{
    MonsterStat monsterStat = null;
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    public override void Heal(float _heal)
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        // 현재 체력과 최대체력 비교
        //monsterStat.HP = 
    }

    IEnumerator CRecovery(float _percent)
    {
        yield return null;  
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 다시 태어날 때 리셋
    /// </summary>
    public void ResetStat()
    {

    }
}
