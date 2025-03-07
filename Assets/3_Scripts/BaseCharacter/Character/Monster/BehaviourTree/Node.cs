using MonsterEnums;

[System.Serializable]
public abstract class Node
{
    public delegate NODESTATES NodeReturn();
    protected NODESTATES nodeState;
    public NODESTATES NodeState { get { return nodeState; } }
    public abstract NODESTATES Evaluate();       
}