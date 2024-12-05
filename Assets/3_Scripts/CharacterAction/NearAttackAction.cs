using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearAttackAction : AttackAction
{
    [SerializeField, Header("공격 변수"), Tooltip("데미지%")] float attackDamageMuiltiplier;
    public override void DoAttack()
    {
        //ParticleSystem particle = 파티클 key로 로드, 후 실행
    }


}
