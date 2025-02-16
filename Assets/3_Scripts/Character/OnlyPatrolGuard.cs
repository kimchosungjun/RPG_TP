using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OnlyPatrolGuard : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav;
    [SerializeField] UtilEnums.PATH_TYPES pathType;
    List<Vector3> pathPositions = null;
    int currentIndex = 0;
    int maxIndex = 0;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
        else
            return;
        PathNode pathNode = SharedMgr.GameCtrlMgr.GetZoneCtrl.GetPathNode(pathType);
        if (pathNode == null) return;
        pathPositions = pathNode.GetPathPositions;
        currentIndex = 0;
        maxIndex = pathPositions.Count;
        StartCoroutine(CPatrol());
    }

    IEnumerator CPatrol()
    {
        nav.SetDestination(pathPositions[currentIndex]);
        while (true)
        {
            if(nav.remainingDistance < 0.5f)
            {
                currentIndex += 1;
                if (currentIndex >= maxIndex)
                {
                    currentIndex = 0;
                    nav.SetDestination(pathPositions[0]);
                }
                else
                {
                    nav.SetDestination(pathPositions[currentIndex]);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
