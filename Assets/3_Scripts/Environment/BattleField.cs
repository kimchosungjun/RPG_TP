using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;
public class BattleField : MonoBehaviour
{
    [SerializeField] protected BaseMonster[] spawnMonsters;
    [SerializeField] protected Transform[] monsterSpawnTransforms;
    [SerializeField] protected float radius;
    protected List<RespawnChecker> respawnMonsters = new List<RespawnChecker>();
    protected int playerLayer = (int) LAYERS.PLAYER;
    protected bool isWaitRespawn = false;
    public float GetRadius { get { return radius; } protected set { radius = value; } }

    protected class RespawnChecker
    {
        float respawnTime;
        float currentTime;
        public BaseMonster monster = null;

        public void SetRespawnInfo(BaseMonster _baseMonster)
        {
            currentTime = 0;
            respawnTime = SharedMgr.TableMgr.GetMonster.GetMonsterInfoTableData(_baseMonster.GetMonsterStat.GetID).respawnTime;
            monster = _baseMonster;
        }
        
        public bool CanRespawn()
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= respawnTime)
                return true;
            return false;
        }
    }

    #region Life Cycle
    protected virtual void Start()
    {
        SetupMonsters();
    }

    public virtual void SetupMonsters()
    {
        int monsterCnt = spawnMonsters.Length;
        for (int i = 0; i < monsterCnt; i++)
        {
            spawnMonsters[i].SetBattleFieldData(this, i, monsterSpawnTransforms[i].position, this.transform.position);
            SpawnMonster(spawnMonsters[i]);
        }
    }

    protected virtual void FixedUpdate()
    {
        CheckRespawnTime();
    }

    public virtual void CheckRespawnTime()
    {
        if (isWaitRespawn == false) return;

        int cnt = respawnMonsters.Count;
        for(int i = cnt-1; i>=0; i--)
        {
            if (respawnMonsters[i].CanRespawn()) 
            {
                RespawnMonster(respawnMonsters[i].monster);
                respawnMonsters.RemoveAt(i);
            }
        }

        cnt = respawnMonsters.Count;
        if (cnt == 0) isWaitRespawn = false;
    }
    #endregion

    #region Trigger 
    protected virtual void OnTriggerEnter(Collider other)
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

    protected virtual void OnTriggerExit(Collider other)
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
    #endregion

    #region Spawn

    public virtual void SpawnMonster(BaseMonster _monster)
    {
        int index = _monster.BattleFieldSpawnIndex;
        _monster.transform.position = monsterSpawnTransforms[index].position;
        _monster.transform.rotation= monsterSpawnTransforms[index].rotation;

        if (_monster.gameObject.activeSelf == false)
            _monster.gameObject.SetActive(true);
    }

    public virtual void RespawnMonster(BaseMonster _monster)
    {
        SpawnMonster(_monster);
        _monster.GetMonsterStatControl.ResetStat();
    }

    public virtual void DeathMonster(GameObject _baseMonsterObject)
    {
        if (_baseMonsterObject.activeSelf == true)
        {
            _baseMonsterObject.SetActive(false);
            RespawnChecker checker = new RespawnChecker();
            checker.SetRespawnInfo(_baseMonsterObject.GetComponent<BaseMonster>());
            respawnMonsters.Add(checker);
            isWaitRespawn = true;
        }
    }

    #endregion
}
