using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enter, Execute(업데이트), Exit 3가지 구성을 Default로 설정
/// </summary>
public class StateMachine 
{
    protected State currentState = null;

    public void ChangeState(State _newState)
    {
        if (currentState == _newState)
            return;
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

    public void InitStateMachine(State _newState)
    {
        currentState = _newState;
        currentState?.Enter();
    }
    public void Execute() { currentState.Execute(); }
}
