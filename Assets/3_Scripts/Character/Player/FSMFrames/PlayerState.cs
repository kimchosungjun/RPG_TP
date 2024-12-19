using UnityEngine;

/// <summary>
///  물리 계산을 하기 위해 FixedUpdate에서 호출될 FixedExecute 추가 선언
///  & 플레이어 조작 스크립트와 전역적으로 호출되는 메서드를 추가로 구현
/// </summary>
public class PlayerState : State
{
    protected Animator anim = null;
    protected PlayerMovementControl characterCtrl = null;
    public PlayerState(PlayerMovementControl _controller) { this.anim = _controller.GetAnim; this.characterCtrl = _controller;}

    public override void Enter() { }
    public override void Execute() { }
    public override void FixedExecute() { }
    public override void Exit() { }

    /// <summary>
    /// 데미지를 입는 상태에선 재정의 하여 사용할 것
    /// </summary>
    public virtual void TakeDamage() { }

    /// <summary>
    /// 죽음 상태 : 어떤 상태든 호출되면 바로 죽음 상태로 전환
    /// </summary>
    public virtual void Death() { }
}
