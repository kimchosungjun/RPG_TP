using MonsterEnums;
using System.Collections;
using UnityEngine;

public abstract class EliteMonster : BaseMonster
{
    #region Status Value (UI & Gauge)

    [SerializeField] protected EliteMonsterStatusUI statusUI = null;
    protected EliteGauge eliteGauge = null;
    public EliteGauge GetEliteGauge { get { return eliteGauge; } }  
    public class EliteGauge
    {
        EliteMonster elite = null;
        float groggyCheckTime = 0;
        float groggyTime;
        float groggyGauge;
        bool countGroggy;

        public EliteGauge(EliteMonster _monster) { elite = _monster; groggyGauge = 100f; countGroggy = true;  groggyTime = 10f; groggyCheckTime = 0; }
        public EliteGauge(EliteMonster _monster, float _time) { elite = _monster; groggyGauge = 100f; countGroggy = true;  groggyTime = _time; groggyCheckTime = 0; }
        public float GetGroggyTime { get { return groggyTime; } }
        public float GetGroggyGauge { get { return groggyGauge; } }
        public void FixedGroggyDecrease()
        {
            if (countGroggy == false)
            {
                groggyCheckTime += Time.fixedDeltaTime;
                if(groggyCheckTime>groggyTime)
                {
                    groggyCheckTime = 0f;
                    countGroggy = true;
                    groggyGauge = 100f;
                }    
                return;
            }
            groggyGauge -= Time.fixedDeltaTime* 2;
            CheckGroggy();
        }

        public void CheckGroggy()
        {
            if (groggyGauge <= 0f)
            {
                elite?.AnnounceGroggyState(groggyTime);
                groggyGauge = 0f;
                countGroggy = false;
            }
        }
        
        public void Hit(float _hitGauge = 1f)
        {
            if (countGroggy == false) return;
            groggyGauge -= _hitGauge;
            CheckGroggy();
        }

        public void Reset()
        {
            groggyCheckTime = 0;
            countGroggy = true;
            groggyGauge = 100f;
        }
    }

    #endregion
    public abstract void AnnounceGroggyState(float _groggyTime);

    // Common 
    public override void AnnounceStatusUI() { statusUI?.UpdateStatusData(); }

    protected override void CreateStates() { }

    public override void Death()
    {
        SetNoneInteractionType();
        GetDropItem();
        BossKillDrop();
    }

    public virtual void BossKillDrop() { }

    #region Battle Field
    public override void AnnounceInMonsterArea()
    {
        base.AnnounceInMonsterArea();
        statusUI.ChangeData(monsterStat, eliteGauge);
    }

    public override void AnnounceOutMonsterArea()
    {
        base.AnnounceOutMonsterArea();
        statusUI.ChangeData();
    }

    public override void EscapeReturnToSpawnPosition() { }
    public void EscapeCalmState() 
    {
        StopAllCoroutines();
        isRecovery = false; 
        isGoOffAggro = false; 
    }

    public bool CheckCalmState() { return (isRecovery==false && isGoOffAggro==false); }

    
    // Go Off Aggro
    public override void GoOffAggro()
    {
        if (isGoOffAggro) return;
        StartCoroutine(CGoOffAggro());
    }

    protected virtual IEnumerator CGoOffAggro()
    {
        isGoOffAggro = true;
        nav.SetDestination(SpawnPosition);
        nav.stoppingDistance = 0;
        while (true)
        {
            if (isGoOffAggro == false) yield break;
            if (nav.remainingDistance < toOriginalStopDistance) break;
            yield return new WaitForFixedUpdate();
        }
        nav.stoppingDistance = toPlayerStopDistance;
        isGoOffAggro = false;
    }
    #endregion
}
