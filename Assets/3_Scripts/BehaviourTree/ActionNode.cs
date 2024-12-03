using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : Node
{
    public delegate E_BTS ActionNodeReturn();
    private ActionNodeReturn btAction = null;

    public ActionNode(ActionNodeReturn btAction) { this.btAction = btAction; }

    public override E_BTS Evaluate()
    {
        switch (btAction())
        {
            case E_BTS.SUCCESS:
                nodeState = E_BTS.SUCCESS;
                return nodeState;
            case E_BTS.FAIL:
                nodeState = E_BTS.FAIL;
                return nodeState;
            case E_BTS.RUNNING:
                nodeState = E_BTS.RUNNING;
                return nodeState;
            default:
                Debug.LogError("잘못된 행동 상태!");
                return E_BTS.FAIL;
        }
    }
}
