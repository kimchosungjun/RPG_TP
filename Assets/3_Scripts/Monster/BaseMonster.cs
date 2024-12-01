using UnityEngine;

// 순수 가상클래스 : 메서드만 들어있는 것

public abstract class BaseMonster : MonoBehaviour
{
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatusUICtrl statusUICtrl = null;

    /// <summary>
    /// 플레이어가 근처에 오면 상태창 활성화
    /// </summary>
    /// <returns></returns>
    public abstract E_BT DetectPlayer();
    public abstract E_BT IdleMovement();
    public abstract E_BT DetectMovement();

    public abstract void Spawn();
    public abstract void Death();
    public abstract void TakeDamage();
    public abstract void Recovery();
}