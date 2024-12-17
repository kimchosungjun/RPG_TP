
using System;

namespace MonsterTableClasses
{
    /************************************************************
    ********** 몬스터 데이터를 저장할 클래스 **************
    ************************************************************/
    public class MonsterClasses { }

    #region MonsterDataClasses : 테이블, 레벨, 스탯, 일반공격, 버프스킬, 공격스킬
    [Serializable]
    public class MonsterInfoTableData
    {
        public int monsterID; 
        public string monsterName;
        public string monsterDescription;
        public string monsterFeature;
    }

    [Serializable]
    public class MonsterDropTableData
    {
        public int dropID;
        public int[] itemIDs;
        public int[] itemDropProbabilities;
        public int dropGold;
        public int dropExp;
        public int minQuantity;
        public int maxQuantity;
        public float[] quantityProbabilities;

        public void SetSize(int _dropCnt, int _quantityCnt)
        {
            itemIDs = new int[_dropCnt];    
            itemDropProbabilities = new int[_dropCnt];  
            quantityProbabilities  = new float[_quantityCnt];
        }
    }

    [Serializable]
    public class NonCombatMonsterStatTableData
    {
        public int monsterID;
        public int monsterLevel;
        public float monsterMaxHP;
        public float monsterSpeed;
        public float monsterBoostSpeed;
        public float monsterDefence;
        public int monsterDropID;
    }


    [Serializable]
    public class CombatMonsterStatTableData : NonCombatMonsterStatTableData
    {
        public float monsterAttack;
        public float monsterCritical;
    }
    #endregion
}