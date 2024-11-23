using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class Node
{
    public delegate E_BT NodeReturn();
    protected E_BT nodeState;
    public E_BT NodeState { get { return nodeState; } }
    public abstract E_BT Evaluate();       
}