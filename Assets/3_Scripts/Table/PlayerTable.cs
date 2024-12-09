using System;
using System.Collections.Generic;
using PlayerEnums;

[Serializable]
public class Info
{
    public int id;
    public byte type;
    public int skill;
    public string prefabs;
}

public partial class PlayerTable : BaseTable
{
    // 플레이어의 정보를 저장한 테이블 데이터 : id가 key 값
    public Dictionary<int, PlayerTableData> playerTableGroup = new Dictionary<int, PlayerTableData>();

    // 누구의 스탯인지 확인하고 레벨에 맞게 스탯을 파싱 : 첫번째 키 값은 플레이어의 id이고 두번째 키 값은 level
    Dictionary<int, Dictionary<int, PlayerStatData>> playerStatDictionary = new Dictionary<int, Dictionary<int, PlayerStatData>>();

    Dictionary<int, Dictionary<int, PlayerAttackData>> playerAttackDataGroup = new Dictionary<int, Dictionary<int, PlayerAttackData>>();

    #region PlayerDataClass

    [Serializable]
    public class PlayerTableData
    {
        public int id;
        public string name;
        public int stat;
        public int attack;
        public int skill;
        public int ultimate;
        public float speed;
        public float dashSpeed;
        public float jumpSpeed;
    }

    [Serializable]
    public class PlayerStatData
    {
        public int level;
        public float hp;
        public float attack;
        public float defence;
        public float critical;
    }

    [Serializable]
    public class PlayerAttackData
    {
        public int level;
        public string name;
        public string description;
        public int needGold;
        public float[] multipliers = new float[3];
        public int[] effects = new int[3];
    }

    #endregion

    #region GetPlayerData
    public PlayerTableData GetPlayerTableData(int _id)
    {
        if (playerTableGroup.ContainsKey(_id))
            return playerTableGroup[_id];
        return null;
    }

    public PlayerAttackData GetPlayerAttackData(TYPEID _typeID, int _level)
    {
        if (playerAttackDataGroup.ContainsKey((int)_typeID))
        {
            if (playerAttackDataGroup[(int)_typeID].ContainsKey(_level))
                return playerAttackDataGroup[(int)_typeID][_level];
        }


        return null;
    }
    #endregion

    public void InitBinary(string _name)
    {
        LoadBinary<Dictionary<int, PlayerTableData>>(_name, ref playerTableGroup);
    }

   
}