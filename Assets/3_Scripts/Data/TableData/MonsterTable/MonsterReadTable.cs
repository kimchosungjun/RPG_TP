using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterTableClasses;

public partial class MonsterTable : BaseTable
{
    /*************************************************************
    *********** 읽어 온 데이터를 Dictionary에 저장 ***************
    *************************************************************/
  
    #region Only Load Monster Data 
    public void InitMonsterTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        for (int row = _startRow; row < reader.row; row++)
        {
            MonsterInfoTableData data = new MonsterInfoTableData();
            if (ReadMonsterTable(reader, data, row, _startCol) == false)
                break;
            monsterInfoTableGroup.Add(data.monsterID, data);
        }
    }

    public void InitMonsterDropTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        MonsterDropTableData data = new MonsterDropTableData();
        if (ReadMonsterDrop(reader, data, _startRow, _startCol) == false)
            return;
        monsterDropGroup.Add(data.dropID, data);
    }

    public void InitNonCombatMonsterStatTableCsv(string _name, int _startRow, int _startCol, int _typeID)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        Dictionary<int, NonCombatMonsterStatTableData> tableDictionary = new Dictionary<int, NonCombatMonsterStatTableData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            NonCombatMonsterStatTableData data = new NonCombatMonsterStatTableData();
            if (ReadNonCombatMonsterDrop(reader, data, row, _startCol) == false)
                break;
            tableDictionary.Add(data.monsterLevel, data);
        }
        nonCombatMonsterStatGroup.Add(_typeID, tableDictionary);
    }
    #endregion

    /*************************************************************
    *********** 지정된 클래스의 데이터를 읽어서 파싱 *************
    *************************************************************/

    #region Link CsvData to ClassData
    protected bool ReadMonsterTable(CSVReader _reader, MonsterInfoTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.monsterID);
        _reader.get(_row, ref _tableData.monsterName);
        _reader.get(_row, ref _tableData.monsterDescription);
        _reader.get(_row, ref _tableData.monsterFeature);
        return true;
    }

    protected bool ReadMonsterDrop(CSVReader _reader, MonsterDropTableData _tableData, int _row, int _col)
    {
        int dropCnt = 0;
        int quantityCnt = 0;

        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.dropID);

        _reader.get(_row, ref dropCnt);
        _reader.get(_row,ref quantityCnt);
        _tableData.SetSize(dropCnt,quantityCnt);

        _reader.get(_row, ref _tableData.itemIDs, _tableData.itemIDs.Length);
        _reader.get(_row, ref _tableData.itemDropProbabilities, _tableData.itemDropProbabilities.Length);
        _reader.get(_row, ref _tableData.dropGold);
        _reader.get(_row, ref _tableData.dropExp);
        _reader.get(_row, ref _tableData.minQuantity);
        _reader.get(_row, ref _tableData.maxQuantity);
        _reader.get(_row, ref _tableData.quantityProbabilities, _tableData.quantityProbabilities.Length);
        return true;
    }
    protected bool ReadNonCombatMonsterDrop(CSVReader _reader, NonCombatMonsterStatTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.monsterID);
        _reader.get(_row, ref _tableData.monsterLevel);
        _reader.get(_row, ref _tableData.monsterMaxHP);
        _reader.get(_row, ref _tableData.monsterSpeed);
        _reader.get(_row, ref _tableData.monsterBoostSpeed);
        _reader.get(_row, ref _tableData.monsterDefence);
        _reader.get(_row, ref _tableData.monsterDropID);
        return true;
    }
    #endregion
}
