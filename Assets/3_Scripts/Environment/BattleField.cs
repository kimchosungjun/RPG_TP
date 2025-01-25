using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;
public class BattleField : MonoBehaviour
{
    // int playerLayer = 1 <<8; : Physics에서는 이 방식 사용
    // Physics는 비트 플래그로 작동하지만 Gameobject의 레이어는 단일 레이어값으로 작동한다.
    [SerializeField] BaseMonster[] spawnMonsters;
    [SerializeField] PathNode[] pathNodes;
    [SerializeField] float respawnTime;

    private void Start()
    {
        //HashSet<int> spawnIndexes = new HashSet<int>();
        //int randomNum = 0;
        //int pathCnt = pathNodes.Length;
        //int monsterCnt = spawnMonsters.Length;
        //for (int i = 0; i < monsterCnt; i++)
        //{
        //    while (true)
        //    {
        //        randomNum = Random.Range(0, pathCnt);
        //        if (spawnIndexes.Contains(randomNum))
        //            continue;
        //        else
        //        {
        //            spawnMonsters[i].Spawn(pathNodes[randomNum].NodePosition);
        //            spawnMonsters[i].MonsterArea = this;
        //            spawnMonsters[i].SetPathNodes(pathNodes);
        //            spawnIndexes.Add(randomNum);
        //            break;
        //        }
        //    }
        //}
    }

    int playerLayer = (int) LAYERS.PLAYER;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            int monsterCnt = spawnMonsters.Length;
            for (int i = 0; i < monsterCnt; i++)
            {
                spawnMonsters[i].AnnounceInMonsterArea();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            int monsterCnt = spawnMonsters.Length;
            for (int i = 0; i < monsterCnt; i++)
            {
                spawnMonsters[i].AnnounceOutMonsterArea();
            }
        }
    }

    public void DeathMonster(GameObject _baseMonsterObject)
    {
        if(_baseMonsterObject.activeSelf==true) _baseMonsterObject.SetActive(false) ;
        StartCoroutine(CRespawn(_baseMonsterObject));
    }

    IEnumerator CRespawn(GameObject _baseMonsterObject)
    {
        BaseMonster baseMonster = _baseMonsterObject.GetComponent<BaseMonster>();   
        yield return new WaitForSeconds(respawnTime);

        bool isSpawn = false;
        int pathCnt = pathNodes.Length;
        for (int i = 0; i < pathCnt; i++)
        {
            if (pathNodes[i].CheckObstacle() == false)
                continue;
            baseMonster.Spawn(pathNodes[i].NodePosition);
            isSpawn = true;
            break;
        }
        if (isSpawn)
            yield break;
        baseMonster.Spawn(pathNodes[0].NodePosition);
    }
}
