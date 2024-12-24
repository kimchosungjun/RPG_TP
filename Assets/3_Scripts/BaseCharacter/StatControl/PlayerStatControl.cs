using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatControl : ActorStatControl
{
    PlayerStat playerStat = null;
    BasePlayer player = null;
    public PlayerStat PlayerStat { get { return playerStat; } set { playerStat = value; } }
    public BasePlayer Player { get { return player;  } set { player = value; } }    
    #region To Do ~~~~~~

    public override void Death() { player.DoDeathState(); }

    public override void Heal(float _heal)
    {
        throw new NotImplementedException();
    }

    public override void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        throw new NotImplementedException();
    }

    public override void TakeDamage(TransferAttackData _attackData)
    {
        playerStat.GetSaveStat.currentHP -= (_attackData.GetAttackValue-playerStat.Defence);
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.UpdateData(playerStat);    
        if(playerStat.GetSaveStat.currentHP<=0.01f) Death();    
    }

    public bool CheckDeathState()
    {
        if (playerStat.GetSaveStat.currentHP <= 0.01f) return true;
        return false;
    }
    #endregion
}



// To do ~~ 
// 캐릭터 변경될 때, 