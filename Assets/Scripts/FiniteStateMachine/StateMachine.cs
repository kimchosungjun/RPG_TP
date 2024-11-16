using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine 
{
    private State currentState = null;

    public StateMachine(State newState) 
    {
        currentState = newState;
        currentState.Enter();
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState)
            return;
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Execute() { currentState.Execute(); }
    public void FixedExecute() { currentState.FixedExecute(); }
}
