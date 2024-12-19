using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerDataLinker : MonoBehaviour
{
    protected PlayerStat stat = null;
    /// <summary>
    /// 공격, 버프에 사용할 플레이어 스탯 데이터가 필요
    /// </summary>
    public abstract void SetPlayerData(PlayerStatControl _statCtrl);
}
