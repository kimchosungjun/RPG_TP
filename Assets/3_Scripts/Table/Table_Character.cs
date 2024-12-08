using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Info
{
    public int id;
    public byte type;
    public int skill;
    public string prefabs;
}

public class Table_Character : Table_Base
{
    public Dictionary<int, Info> Dictionary = new Dictionary<int, Info>();

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
            Info info = new Info();
            if (Read(reader, info, row, _startCol) == false)
                break;
            Dictionary.Add(info.id, info);
            Debug.Log(info.id);
        }
    }

    protected bool Read(CSVReader _reader, Info _info, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false)
            return false;

        _reader.get(_row, ref _info.id);
        _reader.get(_row, ref _info.type);
        _reader.get(_row, ref _info.skill);
        _reader.get(_row, ref _info.prefabs);
        return true;
    }
}