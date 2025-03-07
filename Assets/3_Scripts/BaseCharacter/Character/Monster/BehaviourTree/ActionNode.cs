using MonsterEnums;
using UnityEngine;

public class ActionNode : Node
{
    public delegate NODESTATES ActionNodeReturn();
    private ActionNodeReturn btAction = null;

    public ActionNode(ActionNodeReturn btAction) { this.btAction = btAction; }

    public override NODESTATES Evaluate()
    {
        switch (btAction())
        {
            case NODESTATES.SUCCESS:
                nodeState = NODESTATES.SUCCESS;
                return nodeState;
            case NODESTATES.FAIL:
                nodeState = NODESTATES.FAIL;
                return nodeState;
            case NODESTATES.RUNNING:
                nodeState = NODESTATES.RUNNING;
                return nodeState;
            default:
                Debug.LogError("잘못된 행동 상태!");
                return NODESTATES.FAIL;
        }
    }
}
