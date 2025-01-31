using MonsterEnums;
using System.Collections;
using UnityEngine;

public class EliteMonster : BaseMonster
{

    [SerializeField] protected EliteMonsterStatusUI statusUI = null;
    protected EliteGauge eliteGauge = new EliteGauge();

    public override void AnnounceStatusUI()
    {
        statusUI.UpdateStatusData();
    }

    protected override void CreateStates() {  }

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
            if(groggyGauge <= 0f)
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
}
