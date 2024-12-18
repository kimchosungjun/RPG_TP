using MonsterEnums;
using System.Collections.Generic;

/// <summary>
/// 하나라도 성공하면 성공 반환 (Or)
/// </summary>
public class Selector : Node
{
    private List<Node> nodes = new List<Node>();

    public Selector(List<Node> btNodes) { this.nodes = btNodes; }
    public override NODESTATES Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case NODESTATES.SUCCESS:
                    nodeState = NODESTATES.SUCCESS;
                    return nodeState;
                case NODESTATES.FAIL:
                    continue;    
                case NODESTATES.RUNNING:
                    nodeState = NODESTATES.RUNNING;
                    return nodeState;
            }
        }
        nodeState = NODESTATES.FAIL;
        return nodeState;
    }
}

