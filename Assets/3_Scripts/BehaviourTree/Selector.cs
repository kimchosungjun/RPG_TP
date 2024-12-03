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
                case E_BTS.SUCCESS:
                    nodeState = E_BTS.SUCCESS;
                    return nodeState;
                case E_BTS.FAIL:
                    continue;    
                case E_BTS.RUNNING:
                    nodeState = E_BTS.RUNNING;
                    return nodeState;
            }
        }
        nodeState = E_BTS.FAIL;
        return nodeState;
    }
}

