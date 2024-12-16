using System;

namespace PlayerTableClasses
{
    /************************************************************
    ******** 플레이어 데이터를 저장할 클래스 *************
    ************************************************************/

    #region PlayerDataClasses : 테이블, 레벨, 스탯, 일반공격, 버프스킬, 공격스킬
    [Serializable]
    public class PlayerTableData
    {
        public int id; // enum
        public string name;
        public float speed;
        public float dashSpeed;
        public float jumpSpeed;
        public int normalAttack; // enum
        public int skill; // enum
        public int ultimate; // enum
        public int stat; // enum
    }

    [Serializable]
    public class PlayerLevelTableData
    {
        public int maxLevel;
        public int actionMaxLevel;
        public int[] needExps;
        public int[] normalAttackLevelupGolds;
        public int[] skillLevelupGolds;
        public int[] ultimateLevelupGolds;

        /// <summary>
        /// 최대레벨과 최대행동 레벨을 파싱한 후에 반드시 호출할 것
        /// </summary>
        public void SetSize()
        {
            needExps = new int[maxLevel - 1];
            normalAttackLevelupGolds = new int[actionMaxLevel - 1];
            skillLevelupGolds = new int[actionMaxLevel - 1];
            ultimateLevelupGolds = new int[actionMaxLevel - 1];
        }
    }

    [Serializable]
    public class PlayerStatTableData
    {
        public int level;
        public float maxHp;
        public float attackValue;
        public float defenceValue;
        public float criticalValue;
        public float attackSpeed;
    }

    [Serializable]
    public class PlayerNormalAttackTableData
    {
        public int level;
        public string name;
        public string description;
        public float[] multipliers;
        public float[] effectMaintainTimes;
        public int[] effects;
        public int particle; // enum

        public void SetSize(int _combo)
        {
            multipliers = new float[_combo];
            effectMaintainTimes = new float[_combo];
            effects = new int[_combo];
        }
    }

    [Serializable]
    public class PlayerBuffSkillTableData
    {
        public int level;
        public string name;
        public string description;
        public float[] multipliers;
        public float coolTime;
        public float effectMaintainTime;
        public int[] useStatTypes; // enum : 사용(계수 비례)하는 스탯
        public int[] effectStatTypes; // enum : 영향을 주는 스탯
        public int[] continuityTypes; // enum : 즉발 여부
        public int particle; // enum
        public void SetSize(int _buffTypeCnt)
        {
            multipliers = new float[_buffTypeCnt];
            useStatTypes = new int[_buffTypeCnt];
            effectStatTypes = new int[_buffTypeCnt];
            continuityTypes = new int[_buffTypeCnt];
        }
    }

    [Serializable]
    public class PlayerAttackSkillTableData
    {
        public int level;
        public string name;
        public string description;
        public float multiplier;
        public float coolTime;
        public float effectMaintainTime;
        public int effectType; // enum : ATTACK_EFFECT_TYPES
        public int particle; // enum
    }
    #endregion

    public class PlayerClasses {  }
}


