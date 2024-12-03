using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Inverter : Node
{
    private Node btNode;
    public Node BTNode { get { return btNode; } }

    public Inverter(Node btNode) { this.btNode = btNode; } 

    public override E_BTS Evaluate()
    {
        switch (btNode.Evaluate())
        {
            case E_BTS.BT_SUCCESS:
                nodeState = E_BTS.BT_FAIL;
                return nodeState;
            case E_BTS.BT_FAIL:
                nodeState = E_BTS.BT_SUCCESS;
                return nodeState;
            case E_BTS.BT_RUNNING:
                nodeState = E_BTS.BT_RUNNING;
                return nodeState;
        }
        Debug.LogError("잘못된 행동 상태!");
        return E_BTS.BT_FAIL;
    }
}
