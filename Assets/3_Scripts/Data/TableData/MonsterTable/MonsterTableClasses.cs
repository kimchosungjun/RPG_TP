
using System;
using System.Diagnostics;

namespace MonsterTableClasses
{
    /************************************************************
    ********** 몬스터 데이터를 저장할 클래스 **************
    ************************************************************/
    public class MonsterTableClasses { }

    #region Info Table
    [Serializable]
    public class MonsterInfoTableData
    {
        public int ID; 
        public string name;
        public string description;
        public string feature;
        public string type;
    }
    #endregion

    #region Drop Table
    [Serializable]
    public class MonsterDropTableData
    {
        public int dropID;
        public int[] itemIDs;
        public float[] itemDropProbabilities;
        public int dropGold;
        public int dropExp;
        public int minQuantity;
        public int maxQuantity;
        public float[] quantityProbabilities;

        public void SetSize(int _dropCnt, int _quantityCnt)
        {
            itemIDs = new int[_dropCnt];    
            itemDropProbabilities = new float[_dropCnt];
            quantityProbabilities  = new float[_quantityCnt];
        }
    }
    #endregion

    #region Non Combat Table
    [Serializable]
    public class NonCombatMonsterStatTableData
    {
        public int ID;
        public float maxHP;
        public float speed;
        public float boostSpeed;
        public float defence;
        public int dropID;
        public float hpIncrease;
        public float defenceIncrease;
        public int startLevel;
    }
    #endregion

    #region Combat Table
    [Serializable]
    public class CombatMonsterStatTableData : NonCombatMonsterStatTableData
    {
        public float attack;
        public float critical;
        public float attackIncrease;
        public float criticalIncrease;
    }
    #endregion

    #region Action Table
    [Serializable]
    public class MonsterAttackTableData
    {
        public int ID;
        public int attribute;
        public float multiplier;
        public float effect;
        public float maintainTime;
        public float coolTime;
        public float defaultDamage;
        public float damageIncrease;
    }

    [Serializable]
    public class MonsterConditionTableData
    {
        public int ID;
        public int attribute;
        public float multiplier;
        public float effect;
        public float maintainTime;
        public float coolTime;
        public float defaultConditionValue;
        public int conditionType;
        public float conditionValueIncrease;
    }

    #endregion
}