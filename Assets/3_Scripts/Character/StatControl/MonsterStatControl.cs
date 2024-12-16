using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatControl : CharacterStatControl
{
    public override void Heal(float _heal)
    {
        throw new System.NotImplementedException();
    }

    public override void Recovery(float _percent)
    {
        throw new System.NotImplementedException();
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
