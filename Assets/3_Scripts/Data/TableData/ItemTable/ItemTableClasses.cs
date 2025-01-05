using System;

namespace ItemTableClassGroup
{
    /************************************************************
    ********** 몬스터 데이터를 저장할 클래스 **************
    ************************************************************/
    public class ItemTableClasses { }

    [Serializable]
    public class ItemTableData
    {
        public int ID;
        public string name;
        public string description;
        public string iconName;
        public string typeIconName;
    }

    [Serializable]
    public class  EtcTableData : ItemTableData
    {
        public int exp;
    }

    [Serializable]
    public class ConsumeTableData : ItemTableData
    {
        public int effect;
        public float effectValue;
        public float maintainTime;
    }

    [Serializable]
    public class WeaponTableData : ItemTableData
    {
        public float attackValue;
        public int additionalEffect;
        public float additionEffectValue;
        public float increaseAttackValue;
        public float increaseAdditionEffectValue;
        public int price;
    }


    [Serializable]
    public class WeaponUpgradeTableData
    {
        public int maxLevel;
        public int[] needExps;
        public int[] needGolds;
        
        public void SetSize()
        {
            needExps = new int[maxLevel - 1];   
            needGolds = new int[maxLevel - 1];  
        }
    }
}