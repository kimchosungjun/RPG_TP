using System.Collections.Generic;
using PlayerEnums;
public partial class PlayerTable: BaseTable
{
    public void SaveBinary(string _name)
    {
        SaveBinary(_name, playerTableGroup);
    }

    public void InitCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name);
        for (int row = _startRow; row < reader.row; row++)
        {
            // 사용 예시 : 참고만 할 것
            //Info info = new Info();
            //if (Read(reader, info, row, _startCol) == false)
            //    break;
            //Dictionary.Add(info.id, info);

            PlayerTableData tableData = new PlayerTableData();
            if (Read(reader, tableData, row, _startCol) == false)
                break;
            playerTableGroup.Add(tableData.id, tableData);
        }
    }

    public void InitAttackCsv(string _name, int _startRow, int _startCol, TYPEID _typeID)
    {
        CSVReader reader = GetCSVReader(_name);
        Dictionary<int, PlayerAttackData> dataGroup = new Dictionary<int, PlayerAttackData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerAttackData tableData = new PlayerAttackData();
            if (Read(reader, tableData, row, _startCol) == false)
                break;
            dataGroup.Add(tableData.level, tableData);
        }
        playerAttackDataGroup.Add((int)_typeID, dataGroup);
    }


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

    protected bool Read(CSVReader _reader, PlayerAttackData _tableData, int _row, int _col)
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
}
