using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerActionControl : MonoBehaviour
{
    protected Animator anim = null;
    protected PlayerStat stat = null; 
    protected PlayerMovementControl movementControl = null;
    protected PlayerStatControl statControl = null;
    /// <summary>
    /// 공격, 버프에 사용할 플레이어 스탯 데이터가 필요
    /// </summary>
    public abstract void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl);

    public abstract void EscapeAttackState();
}
