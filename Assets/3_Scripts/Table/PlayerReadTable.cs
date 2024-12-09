using System;
using System.Collections.Generic;
using PlayerEnums;
public partial class PlayerTable: BaseTable
{
    #region Save & Load BinaryData
    public void InitBinary<T>(string _name, T _t)
    {
        LoadBinary<T>(_name, ref _t);
    }

    public void SaveBinary<T>(string _name, T _t)  
    {
        SaveBinary(_name, _t);
    }
    #endregion

    #region Load CsvData
    public void InitCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name);
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerTableData tableData = new PlayerTableData();
            if (Read(reader, tableData, row, _startCol) == false)
                break;
            playerTableGroup.Add(tableData.id, tableData);
        }
    }

    public void InitAttackCsv(string _name, int _startRow, int _startCol, TYPEID _typeID)
    {
        CSVReader reader = GetCSVReader(_name);
        Dictionary<int, PlayerNormalAttackData> dataGroup = new Dictionary<int, PlayerNormalAttackData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerNormalAttackData tableData = new PlayerNormalAttackData();
            if (Read(reader, tableData, row, _startCol) == false)
                break;
            dataGroup.Add(tableData.level, tableData);
        }
        playerAttackDataGroup.Add((int)_typeID, dataGroup);
    }

    #endregion

    #region Link CsvData to ClassData
    protected bool Read(CSVReader _reader, PlayerTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.id);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.stat);
        _reader.get(_row, ref _tableData.attack);
        _reader.get(_row, ref _tableData.skill);
        _reader.get(_row, ref _tableData.ultimate);
        _reader.get(_row, ref _tableData.speed);
        _reader.get(_row, ref _tableData.dashSpeed);
        _reader.get(_row, ref _tableData.jumpSpeed);
        return true;
    }

    protected bool Read(CSVReader _reader, PlayerNormalAttackData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.needGold);
        _reader.get(_row, ref _tableData.multipliers, 3);
        _reader.get(_row, ref _tableData.effects, 3);
        return true;
    }
    #endregion
}
