using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatCtrl : CharacterStatCtrl
{
    [SerializeField] PlayerStat playerStat = null;
    public PlayerStat Stat { get { return playerStat; } }

    public PlayerStatCtrl(PlayerStat _playerStat) { playerStat = _playerStat; }

    #region To Do ~~~~~~
    public override void Heal(float _heal)
    {
        throw new NotImplementedException();
    }

    public override void Recovery(float _percent)
    {
        throw new NotImplementedException();
    }

    public override void TakeDamage(float _damage)
    {
           
    }
    #endregion
}
