using System;

namespace PlayerTableClassGroup
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
        public int defaultHP;
        public int defaultAttack;
        public int defaultDefence;
        public float defaultCritical;
        public float defaultAttackSpeed;
        public int increaseHP;
        public int increaseAttack;
        public int increaseDefence;
        public float increaseCritical;  
        public float increaseAttackSpeed;
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

        public void SetSize()
        {
            needExps = new int[maxLevel - 1];
            normalAttackLevelupGolds = new int[actionMaxLevel - 1];
            skillLevelupGolds = new int[actionMaxLevel - 1];
            ultimateLevelupGolds = new int[actionMaxLevel - 1];
        }
    }

    [Serializable]
    public class PlayerNormalAttackTableData
    {
        public int id;
        public int level;
        public int combo;
        public string name;
        public string description;
        public float[] multipliers;
        public float[] effectMaintainTimes;
        public int[] effects;
        public int particle; // enum

        public void SetSize()
        {
            multipliers = new float[combo];
            effectMaintainTimes = new float[combo];
            effects = new int[combo];
        }
    }

    [Serializable]
    public class PlayerConditionSkillTableData
    {
        public int id;
        public int level;
        public int combo;
        public string name;
        public string description;
        public float[] multipliers;
        public float coolTime;
        public float effectMaintainTime;
        public int[] attributeStatTypes; // enum : 사용(계수 비례)하는 스탯
        public int[] effectStatTypes; // enum : 영향을 주는 스탯
        public int[] continuityTypes; // enum : 즉발 여부
        public float[] defaultValues;
        public int[] applyType; // Use Multiplier
        public int[] partyType;
        public int particle; // enum
        public void SetSize()
        {
            multipliers = new float[combo];
            attributeStatTypes = new int[combo];
            effectStatTypes = new int[combo];
            defaultValues = new float[combo];
            continuityTypes = new int[combo];
            applyType = new int[combo]; 
            partyType = new int[combo];
        }
    }

    [Serializable]
    public class PlayerAttackSkillTableData
    {
        public int id;
        public int level;
        public int combo;
        public string name;
        public string description;
        public float[] multiplier;
        public float coolTime;
        public float[] effectMaintainTime;
        public int[] effectType; // enum : ATTACK_EFFECT_TYPES
        public int particle; // enum
        public int[] defaultValues;

        public void SetSize()
        {
            multiplier = new float[combo];
            effectMaintainTime = new float[combo];
            effectType = new int[combo];
            defaultValues = new int[combo];
        }
    }
    #endregion

    public class PlayerTableClasses {  }
}


