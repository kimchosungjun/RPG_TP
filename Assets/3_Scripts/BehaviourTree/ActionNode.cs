using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : Node
{
    public delegate E_BT ActionNodeReturn();
    private ActionNodeReturn btAction = null;

    public ActionNode(ActionNodeReturn btAction) { this.btAction = btAction; }

    public override E_BT Evaluate()
    {
        switch (btAction())
        {
            case E_BT.BT_SUCCESS:
                nodeState = E_BT.BT_SUCCESS;
                return nodeState;
            case E_BT.BT_FAIL:
                nodeState = E_BT.BT_FAIL;
                return nodeState;
            case E_BT.BT_RUNNING:
                nodeState = E_BT.BT_RUNNING;
                return nodeState;
            default:
                Debug.LogError("잘못된 행동 상태!");
                return E_BT.BT_FAIL;
        }
    }
}
