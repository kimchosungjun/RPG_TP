using UnityEngine;
using MonsterEnums;

public class Inverter : Node
{
    private Node btNode;
    public Node BTNode { get { return btNode; } }

    public Inverter(Node btNode) { this.btNode = btNode; } 

    public override NODESTATES Evaluate()
    {
        switch (btNode.Evaluate())
        {
            case NODESTATES.SUCCESS:
                nodeState = NODESTATES.FAIL;
                return nodeState;
            case NODESTATES.FAIL:
                nodeState = NODESTATES.SUCCESS;
                return nodeState;
            case NODESTATES.RUNNING:
                nodeState = NODESTATES.RUNNING;
                return nodeState;
        }
        Debug.LogError("잘못된 행동 상태!");
        return NODESTATES.FAIL;
    }
}
