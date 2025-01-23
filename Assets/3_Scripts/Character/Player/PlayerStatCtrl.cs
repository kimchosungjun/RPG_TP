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

    /// <summary>
    /// Return All Save Stat : Use for Update Save Stat
    /// </summary>
    /// <returns></returns>
    public List<PlayerSaveStat> GetAllSaveStat()
    {
        int keyCnt = playerStatGroup.Keys.Count;
        List<int> keys = new List<int>(playerStatGroup.Keys);
        List<PlayerSaveStat> result = new List<PlayerSaveStat>();

        for(int i=0; i<keyCnt; i++)
        {
            result.Add(playerStatGroup[keys[i]].GetSaveStat);
        }

        return result;
    }
}
