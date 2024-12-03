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

    public override E_BTS Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case E_BTS.SUCCESS:
                    continue;
                case E_BTS.FAIL:
                    nodeState = E_BTS.FAIL;
                    return nodeState;
                case E_BTS.RUNNING:
                    nodeState = E_BTS.RUNNING;
                    return nodeState;
            }
        }
        nodeState = E_BTS.SUCCESS;
        return nodeState;
    }
}
