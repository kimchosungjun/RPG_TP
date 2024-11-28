using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나라도 성공하면 성공 반환 (Or)
/// </summary>
public class Selector : Node
{
    private List<Node> nodes = new List<Node>();

    public Selector(List<Node> btNodes) { this.nodes = btNodes; }
    public override E_BT Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case E_BT.BT_SUCCESS:
                    nodeState = E_BT.BT_SUCCESS;
                    return nodeState;
                case E_BT.BT_FAIL:
                    continue;    
                case E_BT.BT_RUNNING:
                    nodeState = E_BT.BT_RUNNING;
                    return nodeState;
            }
        }
        nodeState = E_BT.BT_FAIL;
        return nodeState;
    }
}

