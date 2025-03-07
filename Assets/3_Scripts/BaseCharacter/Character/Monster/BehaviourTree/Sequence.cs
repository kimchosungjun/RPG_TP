using MonsterEnums;
using System.Collections.Generic;

/// <summary>
/// 하나라도 실패하면 실패 반환 (And)
/// </summary>
public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    public Sequence(List<Node> btNodes) { this.nodes = btNodes; }

    public override NODESTATES Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case NODESTATES.SUCCESS:
                    continue;
                case NODESTATES.FAIL:
                    nodeState = NODESTATES.FAIL;
                    return nodeState;
                case NODESTATES.RUNNING:
                    nodeState = NODESTATES.RUNNING;
                    return nodeState;
            }
        }
        nodeState = NODESTATES.SUCCESS;
        return nodeState;
    }
}
