using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine 
{
    private State currentState = null;

    public StateMachine(State _newState) 
    {
        currentState = _newState;
        currentState.Enter();
    }

    public void ChangeState(State _newState)
    {
        if (currentState == _newState)
            return;
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

    public void Execute() { currentState.Execute(); }
    public void FixedExecute() { currentState.FixedExecute(); }
}
