using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어는 물리 작용을 하기에 FixedUpdate에서 호출될 FixedExecute 추가
/// </summary>
public class PlayerStateMachine : StateMachine
{
    public void FixedExecute() { currentState.FixedExecute(); }
}
