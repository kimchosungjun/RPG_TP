using CameraTableClassGroup;
using ItemTableClassGroup;
using UnityEngine;

public partial class CameraTable : BaseTable
{
    /*******************************************************
    ************* Save Data In Dictionary ******************
    ********************************************************/

    #region Only Load Camera Data 

    public void InitCameraShakeTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.ITEM);
        for (int row = _startRow; row < reader.row; row++)
        {
            CameraShakeTableData data = new CameraShakeTableData();
            if (ReadCameraShakeTable(reader, data, row, _startCol) == false)
                break;
            if (shakeDataGroup.ContainsKey(data.cameraID) == false)
                shakeDataGroup.Add(data.cameraID, data);
        }
    }

    #endregion

    /*************************************************
    ************** Data Load & Parse *****************
    **************************************************/

    #region Link CsvData to ClassData

    protected bool ReadCameraShakeTable(CSVReader _reader, CameraShakeTableData  _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.cameraID);
        _reader.get(_row, ref _tableData.startDelay);
        _reader.get(_row, ref _tableData.shakeTime);
        _reader.get(_row, ref _tableData.shakeMagnitudeX);
        _reader.get(_row, ref _tableData.shakeMagnitudeY);
        _reader.get(_row, ref _tableData.shakeSpeed);
        _reader.get(_row, ref _tableData.damping);
        _reader.get(_row, ref _tableData.maxShakeCount);
        _reader.get(_row, ref _tableData.localSetting);
        return true;
    }

    #endregion
}
