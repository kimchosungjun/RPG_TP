using UnityEngine;
using MonsterEnums;

// 순수 가상클래스 : 메서드만 들어있는 것

public abstract class BaseMonster : MonoBehaviour
{
    [SerializeField] protected Animator anim = null;
 

    /// <summary>
    /// 플레이어가 근처에 오면 상태창 활성화
    /// </summary>
    /// <returns></returns>
    public abstract BTS DetectPlayer();
    public abstract BTS IdleMovement();
    public abstract BTS DetectMovement();

    public abstract void Spawn();
    public abstract void Death();
    public abstract void TakeDamage();
    public abstract void Recovery();

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void FixedUpdate() { } 
}