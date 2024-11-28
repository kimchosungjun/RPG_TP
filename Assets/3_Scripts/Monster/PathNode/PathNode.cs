using UnityEngine;

public class PathNode 
{ 
    Vector3 nodePosition = Vector3.zero;
    public Vector3 NodePosition { get { return nodePosition; } }    

    public PathNode(Vector3 _nodePosition)
    {
        this.nodePosition = _nodePosition;
    }
}
