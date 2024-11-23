using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 순수 가상클래스 : 메서드만 들어있는 것


public abstract class BaseMonster : MonoBehaviour
{
    protected MonsterData monsterData;

    public abstract void Movement();
    public abstract void Hit();
    public abstract void Attack();
}


public class NormalMonster : BaseMonster
{
    public override void Movement() { }
    public override void Hit() { }
    public override void Attack() { }
}

public class BossMonster : BaseMonster
{
    public override void Movement() { }
    public override void Hit() { }
    public override void Attack() { }
    public virtual void SpecialAttack() { }
}



public class MonsterData
{
    public float moveSpeed;
    public float hp;
    public float defence;
    public float damage;

    public MonsterData() { }

    public MonsterData(float _moveSpeed, float _hp, float _defence, float _damage)
    {
        moveSpeed = _moveSpeed;
        hp = _hp;
        defence = _defence;
        damage = _damage;
    }
}
