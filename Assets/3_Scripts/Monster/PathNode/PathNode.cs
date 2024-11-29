using UnityEngine;

[System.Serializable]   
public class PathNode 
{
    [SerializeField] Vector3 halfDetectSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] Vector3 nodePosition = Vector3.zero;
    public Vector3 NodePosition { get { return nodePosition; } }    

    public PathNode(Vector3 _nodePosition)
    {
        this.nodePosition = _nodePosition;
    }

    /// <summary>
    /// 방해물이 있으면 True, 없으면 False 반환
    /// </summary>
    /// <returns></returns>
    public bool CheckObstacle()
    {
        Collider[] colliders = Physics.OverlapBox(nodePosition + Vector3.up * 0.75f, halfDetectSize);
        if (colliders.Length == 0)
            return true;
        return false;
    }
}
