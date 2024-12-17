
using System;

namespace MonsterTableClasses
{
    /************************************************************
    ********** 몬스터 데이터를 저장할 클래스 **************
    ************************************************************/
    public class MonsterClasses { }

    #region MonsterDataClasses : 테이블, 레벨, 스탯, 일반공격, 버프스킬, 공격스킬
    [Serializable]
    public class MonsterTableData
    {
        public int monsterType; // 전투, 비전투 스탯을 나누기 위함
        public int id; // enum
        public string name;
        public int statID;
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

    #endregion
}