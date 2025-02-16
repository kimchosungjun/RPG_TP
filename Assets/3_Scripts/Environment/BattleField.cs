using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;
public class BattleField : MonoBehaviour
{
    [SerializeField] MonsterPrefabName monsterPrefab;
    [SerializeField] protected Transform monsterGroupParent;
    [SerializeField] protected Transform[] monsterSpawnTransforms;
    [SerializeField] protected float radius;
    protected List<RespawnChecker> respawnMonsters = new List<RespawnChecker>();
    protected List<BaseMonster> spawnMonsters = new List<BaseMonster>();
    protected int playerLayer = (int) LAYERS.PLAYER;
    protected bool isWaitRespawn = false;
    public float GetRadius { get { return radius; } protected set { radius = value; } }

    #region Local Data : (Class, Enum)
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

    protected enum MonsterPrefabName
    {
        P_Mon_Chest,
        P_Mon_HornSlime,
        P_Mon_RedDragon,
        P_Mon_Slime,
        P_Mon_Virus
    }

    #endregion

    #region Life Cycle
    protected virtual void Start()
    {
        CapsuleCollider coll = GetComponent<CapsuleCollider>();
        if (coll!=null)
            radius = coll.radius;
        SetupMonsters();
    }

    public virtual void SetupMonsters()
    {
        int monsterCnt = monsterSpawnTransforms.Length;
        string prefabPath = "Monsters/"+Enums.GetEnumString<MonsterPrefabName>(monsterPrefab);
        for (int i = 0; i < monsterCnt; i++)
        {
            // To Do revise
            Transform monsterTransform = SharedMgr.ResourceMgr.LoadResource<Transform>(prefabPath);
            if (monsterTransform == null)
                continue;
            GameObject go = Instantiate(monsterTransform.gameObject);
            go.transform.SetParent(monsterGroupParent);
            BaseMonster monster = go.GetComponent<BaseMonster>();   
            monster.SetBattleFieldData(this, i, monsterSpawnTransforms[i].position, this.transform.position);
            spawnMonsters.Add(monster);
            SpawnMonster(monster);
        }
    }

    public void AnnounceAllPlayerDeath()
    {
        int cnt = spawnMonsters.Count;
        for(int i=0; i<cnt; i++)
        {
            spawnMonsters[i].AnnounceAllPlayerDeath();
        }
    }

    public void AnnounceAllPlayerRevival()
    {
        int cnt = spawnMonsters.Count;
        for (int i = 0; i < cnt; i++)
        {
            spawnMonsters[i].AnnounceAllPlayerRevival();
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
            int monsterCnt = spawnMonsters.Count;
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
            int monsterCnt = spawnMonsters.Count;
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

        Collider[] colls = Physics.OverlapSphere(transform.position, radius, 1 << (int)LAYERS.PLAYER);
        if (colls.Length == 0)
            _monster.RespawnBattleFieldData(false);
        else
            _monster.RespawnBattleFieldData(true);
    }

    public virtual void RespawnMonster(BaseMonster _monster)
    {
        _monster.GetMonsterStatControl.ResetStat();
        SpawnMonster(_monster);
    }

    public virtual void DeathMonster(GameObject _baseMonsterObject)
    {
        if (_baseMonsterObject.activeSelf == true)
        {
            RespawnChecker checker = new RespawnChecker();
            checker.SetRespawnInfo(_baseMonsterObject.GetComponent<BaseMonster>());
            respawnMonsters.Add(checker);
            isWaitRespawn = true;
            _baseMonsterObject.SetActive(false);
        }
    }

    #endregion
}
