using MonsterEnums;

[System.Serializable]
public abstract class Node
{
    public delegate BTS NodeReturn();
    protected BTS nodeState;
    public BTS NodeState { get { return nodeState; } }
    public abstract BTS Evaluate();       
}