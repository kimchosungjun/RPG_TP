using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected CharacterCtrl controller = null;

    public abstract void Enter();
    public abstract void Execute();
    public abstract void FixedExecute();
    public abstract void Exit();

    public State(CharacterCtrl _controller) { this.controller = _controller; }
}
