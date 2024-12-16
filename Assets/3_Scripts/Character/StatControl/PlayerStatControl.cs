using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatControl : CharacterStatControl
{
    [SerializeField] PlayerStat playerStat = null;
    public PlayerStat PlayerStat { get { return playerStat; } set { playerStat = value; } }

    #region To Do ~~~~~~
    public override void Heal(float _heal)
    {
        throw new NotImplementedException();
    }

    public override void Recovery(float _percent)
    {
        throw new NotImplementedException();
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
           
    }
    #endregion
}
