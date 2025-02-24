using ItemTableClassGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemTable : BaseTable
{
    /*******************************************************************
    *********** 읽어 온 데이터를 Dictionary에 저장 ***************
    *******************************************************************/

    #region Only Load Item Data 


    public void InitWeaponUpgradeTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.ITEM);
        WeaponUpgradeTableData data = new WeaponUpgradeTableData();
        if (ReadWeaponUpgradeTable(reader, data, _startRow, _startCol) == false)
            return;
        weaponUpgradeData = data;
    }

    public void InitEtcTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.ITEM);
        for (int row = _startRow; row < reader.row; row++)
        {
            EtcTableData data = new EtcTableData();
            if (ReadEtcTable(reader, data, row, _startCol) == false)
                break;
            if (etcDataGroup.ContainsKey(data.ID) == false)
                etcDataGroup.Add(data.ID, data);
        }
    }

    public void InitConsumeTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.ITEM);
        for (int row = _startRow; row < reader.row; row++)
        {
            ConsumeTableData data = new ConsumeTableData();
            if (ReadConsumeTable(reader, data, row, _startCol) == false)
                break;
            if (consumeDataGroup.ContainsKey(data.ID) == false)
                consumeDataGroup.Add(data.ID, data);
        }
    }


    public void InitWeaponTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.ITEM);
        for (int row = _startRow; row < reader.row; row++)
        {
            WeaponTableData data = new WeaponTableData();
            if (ReadWeaponTable(reader, data, row, _startCol) == false)
                break;
            if (weaponDataGroup.ContainsKey(data.ID) == false)
                weaponDataGroup.Add(data.ID, data);
        }
    }


    #endregion

    /********************************************************************
    *********** 지정된 클래스의 데이터를 읽어서 파싱 ************
    ********************************************************************/

    #region Link CsvData to ClassData

    protected bool ReadWeaponUpgradeTable(CSVReader _reader, WeaponUpgradeTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.maxLevel);
        _tableData.SetSize();
        _reader.get(_row, ref _tableData.needGolds, _tableData.needGolds.Length);
        _reader.get(_row, ref _tableData.needExps, _tableData.needExps.Length);
        return true;
    }

    protected bool ReadEtcTable(CSVReader _reader, EtcTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.itemType);
        _reader.get(_row, ref _tableData.exp);
        _reader.get(_row, ref _tableData.fileName);
        _reader.get(_row, ref _tableData.atlasName);
        return true;
    }

    protected bool ReadConsumeTable(CSVReader _reader, ConsumeTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.itemType);
        _reader.get(_row, ref _tableData.multiplier);
        _reader.get(_row, ref _tableData.maintainTime);
        _reader.get(_row, ref _tableData.attributeStat);
        _reader.get(_row, ref _tableData.effectStat);
        _reader.get(_row, ref _tableData.duration);
        _reader.get(_row, ref _tableData.defaultValue);
        _reader.get(_row, ref _tableData.applyStatType);
        _reader.get(_row, ref _tableData.partyType);
        _reader.get(_row, ref _tableData.fileName);
        _reader.get(_row, ref _tableData.atlasName);
        return true;
    }

    protected bool ReadWeaponTable(CSVReader _reader, WeaponTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.ID);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.itemType);
        _reader.get(_row, ref _tableData.attackValue);
        _reader.get(_row, ref _tableData.additionalEffect);
        _reader.get(_row, ref _tableData.additionEffectValue);
        _reader.get(_row, ref _tableData.increaseAttackValue);
        _reader.get(_row, ref _tableData.increaseAdditionEffectValue);
        _reader.get(_row, ref _tableData.price);
        _reader.get(_row, ref _tableData.atlasName);
        _reader.get(_row, ref _tableData.fileName);
        _reader.get(_row, ref _tableData.weaponType);
        return true;
    }

    #endregion

}
