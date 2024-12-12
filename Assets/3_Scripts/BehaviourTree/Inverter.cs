using UnityEngine;
using MonsterEnums;

public class Inverter : Node
{
    private Node btNode;
    public Node BTNode { get { return btNode; } }

    public Inverter(Node btNode) { this.btNode = btNode; } 

    public override BTS Evaluate()
    {
        switch (btNode.Evaluate())
        {
            case BTS.SUCCESS:
                nodeState = BTS.FAIL;
                return nodeState;
            case BTS.FAIL:
                nodeState = BTS.SUCCESS;
                return nodeState;
            case BTS.RUNNING:
                nodeState = BTS.RUNNING;
                return nodeState;
        }
        Debug.LogError("잘못된 행동 상태!");
        return BTS.FAIL;
    }
}
