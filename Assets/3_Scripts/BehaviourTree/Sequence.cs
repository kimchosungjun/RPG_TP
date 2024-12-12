using MonsterEnums;
using System.Collections.Generic;

/// <summary>
/// 하나라도 실패하면 실패 반환 (And)
/// </summary>
public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    public Sequence(List<Node> btNodes) { this.nodes = btNodes; }

    public override BTS Evaluate()
    {
        int listCnt = nodes.Count;
        for (int i = 0; i < listCnt; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case BTS.SUCCESS:
                    continue;
                case BTS.FAIL:
                    nodeState = BTS.FAIL;
                    return nodeState;
                case BTS.RUNNING:
                    nodeState = BTS.RUNNING;
                    return nodeState;
            }
        }
        nodeState = BTS.SUCCESS;
        return nodeState;
    }
}
