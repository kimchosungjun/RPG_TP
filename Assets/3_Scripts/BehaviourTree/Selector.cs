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
    public override E_BTS Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case E_BTS.BT_SUCCESS:
                    nodeState = E_BTS.BT_SUCCESS;
                    return nodeState;
                case E_BTS.BT_FAIL:
                    continue;    
                case E_BTS.BT_RUNNING:
                    nodeState = E_BTS.BT_RUNNING;
                    return nodeState;
            }
        }
        nodeState = E_BTS.BT_FAIL;
        return nodeState;
    }
}

