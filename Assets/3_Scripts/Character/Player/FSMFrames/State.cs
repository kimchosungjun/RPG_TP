using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public abstract void Enter();
    public abstract void Exit();
    public virtual void Execute() { }
    public virtual void FixedExecute() { }
}
