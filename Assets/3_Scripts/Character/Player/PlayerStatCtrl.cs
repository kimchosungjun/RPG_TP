using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatCtrl : MonoBehaviour
{
    Dictionary<int, PlayerStat> playerStatGroup = new Dictionary<int, PlayerStat>();

    public PlayerStat GetPlayerStat(int _id)
    {
        if (playerStatGroup.ContainsKey(_id) == false) return null;
        return playerStatGroup[_id];
    }

    public void AddPlayerStat(PlayerStat _stat)
    {
        if (_stat == null) return;
        if (playerStatGroup.ContainsKey(_stat.GetSaveStat.playerTypeID))
            return;
        playerStatGroup.Add(_stat.GetSaveStat.playerTypeID, _stat);
    }
}
