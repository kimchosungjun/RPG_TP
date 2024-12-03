using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class Node
{
    public delegate E_BTS NodeReturn();
    protected E_BTS nodeState;
    public E_BTS NodeState { get { return nodeState; } }
    public abstract E_BTS Evaluate();       
}