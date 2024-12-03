using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    // 이름
    // 기본 이동 속도
    // hp
    // 공격력
    // 방어력

    
    [SerializeField] protected int playerType; // => 굳이 나눌 필요는 없어보임.
    [SerializeField] protected float playerNeedEXP;
}
