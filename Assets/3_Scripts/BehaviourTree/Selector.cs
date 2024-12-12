using MonsterEnums;
using System.Collections.Generic;

/// <summary>
/// 하나라도 성공하면 성공 반환 (Or)
/// </summary>
public class Selector : Node
{
    private List<Node> nodes = new List<Node>();

    public Selector(List<Node> btNodes) { this.nodes = btNodes; }
    public override BTS Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case BTS.SUCCESS:
                    nodeState = BTS.SUCCESS;
                    return nodeState;
                case BTS.FAIL:
                    continue;    
                case BTS.RUNNING:
                    nodeState = BTS.RUNNING;
                    return nodeState;
            }
        }
        nodeState = BTS.FAIL;
        return nodeState;
    }
}

