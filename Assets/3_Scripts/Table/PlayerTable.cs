using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;
using Unity.VisualScripting;

[Serializable]
public class Info
{
    public int id;
    public byte type;
    public int skill;
    public string prefabs;
}

public class PlayerTable : Table_Base
{
    public Dictionary<int, Info> Dictionary = new Dictionary<int, Info>();

    // 누구의 스탯인지 확인하고 레벨에 맞게 스탯을 파싱 : Stat의 key값은 level
    Dictionary<STAT, Dictionary<int, PlayerStatData>> playerStatDictionary = new Dictionary<STAT, Dictionary<int, PlayerStatData>>();


    #region PlayerDataClass
    [Serializable]
    public class PlayerTableData
    {
        [SerializeField] int id;
        [SerializeField] string name;
        [SerializeField] STAT stat;
        [SerializeField] ATTACK attack;
        [SerializeField] SKILL skill;
        [SerializeField] ULTIMATE ultimate;
        [SerializeField] float speed;
        [SerializeField] float dashSpeed;
        [SerializeField] float jumpSpeed;

        public int ID { get { return id;  } set { id = value; } }
        public string Name { get { return name; }set { name = value; } }
        public float Speed { get { return speed; } set { speed = value; } }
        public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
        public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    
        public void SetEnumValue<T>(int _idx) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), _idx))
            {
                
            }
            //switch (_idx)
            //{
            //    case 0:
            //        stat = STAT.WARRIOR;
            //        break;
            //    case 1:
            //        stat = STAT.ARCHER;
            //        break;
            //    case 2:
            //        stat = STAT.MAGE;
            //        break;
            //}
             
        }
    }


    [Serializable]
    public class PlayerStatData
    {
        #region StatValue
        [SerializeField] int level;
        [SerializeField] float hp;
        [SerializeField] float attack;
        [SerializeField] float defence;
        [SerializeField] float critical;

        #endregion

        #region Property
        public int Level { get { return level; } set { level = value; } }
        public float Hp { get { return hp; } set{ hp = value; } }
        public float Attack { get { return attack; } set { attack = value; } }
        public float Defence { get { return defence; } set { defence = value; } }
        public float Critical { get { return critical; } set { critical = value; } }

        #endregion

        #region Creator
        public PlayerStatData() { }
        public PlayerStatData(PlayerStatData _playerStat)
        {
            level = _playerStat.level;
            hp = _playerStat.hp;
            attack = _playerStat.attack;
            defence = _playerStat.defence;
            critical = _playerStat.critical;
        }
        #endregion
    }

    #endregion


    public Info Get(int _id)
    {
        if (Dictionary.ContainsKey(_id))
            return Dictionary[_id];
        return null;
    }

    public void Init_Binary(string _name)
    {
        Load_Binary<Dictionary<int, Info>>(_name, ref Dictionary);
    }

    public void Save_Binary(string _name)
    {
        Save_Binary(_name, Dictionary);
    }

    public void Init_Csv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name);
        for (int row = _startRow; row < reader.row; row++)
        {
            // 사용 예시 : 참고만 할 것
            //Info info = new Info();
            //if (Read(reader, info, row, _startCol) == false)
            //    break;
            //Dictionary.Add(info.id, info);
        
            
        }
    }

    protected bool Read(CSVReader _reader, Info _info, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false)
            return false;

        // csv에 적힌 순서대로 파싱한다.
        _reader.get(_row, ref _info.id);
        _reader.get(_row, ref _info.type);
        _reader.get(_row, ref _info.skill);
        _reader.get(_row, ref _info.prefabs);
        return true;
    }

    protected bool Read<T>(CSVReader _reader, T _t, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        //_reader.get(_row, ref _t);
        return true;
    }
}