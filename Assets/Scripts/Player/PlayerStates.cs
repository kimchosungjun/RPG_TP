using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public abstract void Init();
    public abstract void Setup();
    public abstract void Execute();
    public virtual void LateExecute() { }
}
