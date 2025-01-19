using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public void FixedExecute() { currentState.FixedExecute(); }
}
