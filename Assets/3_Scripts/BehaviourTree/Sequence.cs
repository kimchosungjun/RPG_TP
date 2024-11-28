using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나라도 실패하면 실패 반환 (And)
/// </summary>
public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    public Sequence(List<Node> btNodes) { this.nodes = btNodes; }

    public override E_BT Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case E_BT.BT_SUCCESS:
                    continue;
                case E_BT.BT_FAIL:
                    nodeState = E_BT.BT_FAIL;
                    return nodeState;
                case E_BT.BT_RUNNING:
                    nodeState = E_BT.BT_RUNNING;
                    return nodeState;
            }
        }
        nodeState = E_BT.BT_SUCCESS;
        return nodeState;
    }
}
