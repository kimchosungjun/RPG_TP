using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    protected CharacterCtrl characterCtrl;
    public PlayerState(CharacterCtrl _controller) { this.characterCtrl = _controller;}

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
