using MonsterEnums;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelector : Node
{
    List<Node> btNodes;

    public RandomSelector(List<Node> _btNodes) { this.btNodes = _btNodes; }

    public override NODESTATES Evaluate()
    {
        int nodeCnt = btNodes.Count;
        int randomIndex = Randoms.GetRandomCnt(0, nodeCnt);
        return btNodes[randomIndex].Evaluate();
    }
}
