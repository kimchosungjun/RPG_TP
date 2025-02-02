using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;

public class ZoneCtrl : MonoBehaviour
{
    [SerializeField] ZONE_TPYES cuurentZone;
    [SerializeField] PathNodeControl pathNodeControl;

    public void ChangeZone(ZONE_TPYES _currentZone)
    {
        if (_currentZone == cuurentZone) return;   
    }    

    public PathNode GetPathNode(PATH_TYPES _pathType) { return pathNodeControl.GetPathNode(_pathType); }
}

[Serializable]
public class PathNodeControl
{
    [SerializeField] List<PathNode> pathNodes;
    
    public PathNode GetPathNode(PATH_TYPES _pathType)
    {
        int pathNodeCnt = pathNodes.Count;
        for(int i=0; i<pathNodeCnt; i++)
        {
            if(_pathType == pathNodes[i].GetPathType)
                return pathNodes[i];    
        }
        return null;
    }
}

[Serializable]
public class PathNode
{
    [SerializeField] PATH_TYPES pathType;
    [SerializeField] List<Vector3> pathPositions;

    public PATH_TYPES GetPathType { get { return pathType; } }
    public List<Vector3> GetPathPositions { get { return pathPositions; } }
}
