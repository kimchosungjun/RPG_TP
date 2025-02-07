using MonsterEnums;
using System.Collections;
using UnityEngine;

public class EliteMonster : BaseMonster
{
    #region Status Value (UI & Gauge)

    [SerializeField] protected EliteMonsterStatusUI statusUI = null;
    protected EliteGauge eliteGauge = new EliteGauge();
    public class EliteGauge
    {
        float groggyGauge = 100f;
        bool countGroggy = true;

        public float GetGroggyGauge { get { return groggyGauge; } }
        public void FixedGroggyDecrease()
        {
            if (countGroggy == false) return;
            groggyGauge -= Time.fixedDeltaTime;
            CheckGroggy();
        }

        public void CheckGroggy()
        {
            if (groggyGauge <= 0f)
            {
                // To Do Announce
                countGroggy = false;
            }
        }

        public void DoNormalAttack()
        {
            groggyGauge -= 10f;
            CheckGroggy();
        }

        public void DoSkill()
        {
            groggyGauge -= 15;
            CheckGroggy();
        }
    }

    #endregion

    // Common 
    public override void AnnounceStatusUI() { statusUI.UpdateStatusData(); }
    
    protected override void CreateStates() { }

    #region Go Off Aggro
    public override void GoOffAggro()
    {
        if (isGoOffAggro) return;
        StartCoroutine(CGoOffAggro());
    }

    IEnumerator CGoOffAggro()
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
        //ChangeAnimation(STATES.IDLE);
    }
    #endregion
}
