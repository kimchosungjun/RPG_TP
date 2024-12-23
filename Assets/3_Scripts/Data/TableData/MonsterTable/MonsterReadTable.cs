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
    public void InitMonsterInfoTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        for (int row = _startRow; row < reader.row; row++)
        {
            MonsterInfoTableData data = new MonsterInfoTableData();
            if (ReadMonsterInfo(reader, data, row, _startCol) == false)
                break;
            monsterInfoTableGroup.Add(data.ID, data);
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

    public void InitMonsterStatTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        for (int row = _startRow; row < reader.row; row++)
        {
            MonsterStatTableData data = new MonsterStatTableData();
            if (ReadMonsterStat(reader, data, row, _startCol) == false)
                break;
            nonCombatMonsterStatGroup.Add(data.ID, data);
        }    
    }

    public void InitMonsterAttackTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        for (int row = _startRow; row < reader.row; row++)
        {
            MonsterAttackTableData data = new MonsterAttackTableData();
            if (ReadMonsterAttackTable(reader, data, row, _startCol) == false)
                break;
            monsterAttackGroup.Add(data.ID, data);
        }
    }

    public void InitMonsterConditionTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.MONSTER);
        for (int row = _startRow; row < reader.row; row++)
        {
            MonsterConditionTableData data = new MonsterConditionTableData();
            if (ReadMonsterConditionTable(reader, data, row, _startCol) == false)
                break;
            monsterConditionGroup.Add(data.ID, data);
        }
    }
    #endregion

    /*************************************************************
    *********** 지정된 클래스의 데이터를 읽어서 파싱 *************
    *************************************************************/

    #region Link CsvData to ClassData
    protected bool ReadMonsterInfo(CSVReader _reader, MonsterInfoTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.feature);
        _reader.get(_row, ref _tableData.type);
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
    
    protected bool ReadMonsterStat(CSVReader _reader, MonsterStatTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.maxHP);
        _reader.get(_row, ref _tableData.speed);
        _reader.get(_row, ref _tableData.boostSpeed);
        _reader.get(_row, ref _tableData.attack);
        _reader.get(_row, ref _tableData.defence);
        _reader.get(_row, ref _tableData.critical);
        _reader.get(_row, ref _tableData.dropID);
        _reader.get(_row, ref _tableData.hpIncrease);
        _reader.get(_row, ref _tableData.attackIncrease);
        _reader.get(_row, ref _tableData.defenceIncrease);
        _reader.get(_row, ref _tableData.criticalIncrease);
        _reader.get(_row, ref _tableData.startLevel);
        return true;
    }

    protected bool ReadMonsterAttackTable(CSVReader _reader, MonsterAttackTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.attribute);
        _reader.get(_row, ref _tableData.multiplier);
        _reader.get(_row, ref _tableData.effect);
        _reader.get(_row, ref _tableData.maintainTime);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.defaultDamage);
        _reader.get(_row, ref _tableData.damageIncrease);
        _reader.get(_row, ref _tableData.startLevel);
        return true;
    }

    protected bool ReadMonsterConditionTable(CSVReader _reader, MonsterConditionTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.attribute);
        _reader.get(_row, ref _tableData.multiplier);
        _reader.get(_row, ref _tableData.effect);
        _reader.get(_row, ref _tableData.maintainTime);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.defaultConditionValue);
        _reader.get(_row, ref _tableData.conditionType);
        _reader.get(_row, ref _tableData.conditionValueIncrease);
        _reader.get(_row, ref _tableData.startLevel);
        return true;
    }
    #endregion
}
