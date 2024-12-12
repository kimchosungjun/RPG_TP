using MonsterEnums;
using UnityEngine;

public class ActionNode : Node
{
    public delegate BTS ActionNodeReturn();
    private ActionNodeReturn btAction = null;

    public ActionNode(ActionNodeReturn btAction) { this.btAction = btAction; }

    public override BTS Evaluate()
    {
        switch (btAction())
        {
            case BTS.SUCCESS:
                nodeState = BTS.SUCCESS;
                return nodeState;
            case BTS.FAIL:
                nodeState = BTS.FAIL;
                return nodeState;
            case BTS.RUNNING:
                nodeState = BTS.RUNNING;
                return nodeState;
            default:
                Debug.LogError("잘못된 행동 상태!");
                return BTS.FAIL;
        }
    }
}
