
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
        public int monsterID; 
        public string monsterName;
        public string monsterDescription;
        public string monsterFeature;
        public int[] monsterLevels;

        public void SetSize(int _size)
        {
            monsterLevels = new int[_size]; 
        }
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
        public int monsterID;
        public int monsterLevel;
        public float monsterMaxHP;
        public float monsterSpeed;
        public float monsterBoostSpeed;
        public float monsterDefence;
        public int monsterDropID;
    }
    #endregion

    #region Combat Table
    [Serializable]
    public class CombatMonsterStatTableData : NonCombatMonsterStatTableData
    {
        public float monsterAttack;
        public float monsterCritical;
    }
    #endregion

    #region Action Table
    public class MonsterActionTableData
    {

    }

    #endregion
}