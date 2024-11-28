using UnityEngine;

// 순수 가상클래스 : 메서드만 들어있는 것

public abstract class BaseMonster : MonoBehaviour
{
    protected Animator anim = null;

    public abstract E_BT DetectPlayer();
    public abstract E_BT IdleMovement();
    public abstract E_BT DetectMovement();

    public abstract void Spawn();
    public abstract void Death();
    public abstract void TakeDamage();
    public abstract void Recovery();
}