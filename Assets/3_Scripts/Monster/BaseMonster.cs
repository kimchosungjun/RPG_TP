using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 순수 가상클래스 : 메서드만 들어있는 것


public abstract class BaseMonster : MonoBehaviour
{
    public abstract void Spawn();
    public abstract void Death();

    public abstract void DetectPlayer();
    public abstract void IdleMovement();
    public abstract void DetectMovement();

    public abstract void Attack();
    public abstract void TakeDamage();
    public abstract void Recovery();
}