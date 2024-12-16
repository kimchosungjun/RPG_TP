using System.Collections.Generic;
using PlayerEnums;
using PlayerTableClasses;
public partial class PlayerTable: BaseTable
{

    /*************************************************************
    ********* Binary 형식의 데이터 저장 & 로드 *************
    *************************************************************/

    #region Save & Load BinaryData
    //public void InitBinary<T>(string _name, T _t)
    //{
    //    LoadBinary<T>(_name, ref _t);
    //}

    //public void SaveBinary<T>(string _name, T _t)  
    //{
    //    SaveBinary(_name, _t);
    //}
    #endregion

    /*************************************************************
    ******** 읽어 온 데이터를 Dictionary에 저장 ************
   *************************************************************/

    #region Only Load Player Data 

    /// <summary>
    /// 플레이어 테이블 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    public void InitPlayerTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerTableData data = new PlayerTableData();
            if (ReadPlayerTable(reader, data, row, _startCol) == false)
                break;
            playerTableGroup.Add(data.id, data);
        }
    }

    /// <summary>
    /// 플레이어 레벨 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    public void InitPlayerLevelTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        PlayerLevelTableData data = new PlayerLevelTableData();
        if (ReadPlayerLevel(reader, data, _startRow, _startCol) == false)
            return;
        levelupTableData = data;
    }

    /// <summary>
    /// 플레이어 스탯 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    public void InitPlayerStatTableCsv(string _name, int _startRow, int _startCol , int _typeID)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        Dictionary<int, PlayerStatTableData> tableDictionary = new Dictionary<int, PlayerStatTableData> ();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerStatTableData data = new PlayerStatTableData();
            if (ReadPlayerStat(reader, data, row, _startCol) == false)
                break;
            tableDictionary.Add(data.level, data);
        }
        playerStatGroup.Add(_typeID, tableDictionary);
    }

    /// <summary>
    /// 플레이어 기본공격 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerNormalAttackTableCsv(string _name, int _startRow, int _startCol, int _typeID, int _comboCnt)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        Dictionary<int, PlayerNormalAttackTableData> tableDictionary = new Dictionary<int, PlayerNormalAttackTableData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerNormalAttackTableData data = new PlayerNormalAttackTableData();
            data.SetSize(_comboCnt);    
            if (ReadPlayerNormalAttack(reader, data, row, _startCol) == false)
                break;
            tableDictionary.Add(data.level, data);
        }
        playerNormalAttackDataGroup.Add(_typeID, tableDictionary);
    }

    /// <summary>
    /// 플레이어 버프스킬 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerBuffSkillTableCsv(string _name, int _startRow, int _startCol, PlayerEnums.BUFF_SKILLS _typeID, int _comboCnt)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        Dictionary<int, PlayerBuffSkillTableData> tableDictionary = new Dictionary<int, PlayerBuffSkillTableData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerBuffSkillTableData data = new PlayerBuffSkillTableData();
            data.SetSize(_comboCnt);
            if (ReadPlayerBuffSkill(reader, data, row, _startCol) == false)
                break;
            tableDictionary.Add(data.level, data);
        }
        playerBuffSkillDataGroup.Add(_typeID, tableDictionary);
    }

    /// <summary>
    /// 플레이어 공격스킬 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerAttackSkillTableCsv(string _name, int _startRow, int _startCol, PlayerEnums.ATTACK_SKILLS _typeID)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        Dictionary<int, PlayerAttackSkillTableData> tableDictionary = new Dictionary<int, PlayerAttackSkillTableData>();
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerAttackSkillTableData data = new PlayerAttackSkillTableData();
            if (ReadPlayerAttackSkill(reader, data, row, _startCol) == false)
                break;
            tableDictionary.Add(data.level, data);
        }
        playerAttackSkillDataGroup.Add(_typeID, tableDictionary);
    }

    #endregion

    /*************************************************************
    ****** 지정된 클래스의 데이터를 읽어서 파싱 **********
    *************************************************************/

    #region Link CsvData to ClassData
    /// <summary>
    /// 플레이어 테이블 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerTable(CSVReader _reader, PlayerTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.id);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.speed);
        _reader.get(_row, ref _tableData.dashSpeed);
        _reader.get(_row, ref _tableData.jumpSpeed);
        _reader.get(_row, ref _tableData.normalAttack);
        _reader.get(_row, ref _tableData.skill);
        _reader.get(_row, ref _tableData.ultimate);
        _reader.get(_row, ref _tableData.stat);
        return true;
    }

    /// <summary>
    /// 플레이어 레벨업 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerLevel(CSVReader _reader, PlayerLevelTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.maxLevel);
        _reader.get(_row, ref _tableData.actionMaxLevel);
        _tableData.SetSize();
        _reader.get(_row, ref _tableData.needExps, _tableData.maxLevel-1);
        _reader.get(_row, ref _tableData.normalAttackLevelupGolds, _tableData.actionMaxLevel-1);
        _reader.get(_row, ref _tableData.skillLevelupGolds, _tableData.actionMaxLevel - 1);
        _reader.get(_row, ref _tableData.ultimateLevelupGolds, _tableData.actionMaxLevel - 1);
        return true;
    }

    /// <summary>
    /// 플레이어 스탯 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerStat(CSVReader _reader, PlayerStatTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.maxHp);
        _reader.get(_row, ref _tableData.attackValue);
        _reader.get(_row, ref _tableData.defenceValue);
        _reader.get(_row, ref _tableData.criticalValue);
        _reader.get(_row, ref _tableData.attackSpeed);
        return true;
    }

    /// <summary>
    /// 플레이어 기본공격 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerNormalAttack(CSVReader _reader, PlayerNormalAttackTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multipliers, _tableData.multipliers.Length);
        _reader.get(_row, ref _tableData.effectMaintainTimes, _tableData.effectMaintainTimes.Length);
        _reader.get(_row, ref _tableData.effects, _tableData.effects.Length);
        _reader.get(_row, ref _tableData.particle);
        return true;
    }

    /// <summary>
    /// 플레이어 버프스킬 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerBuffSkill(CSVReader _reader, PlayerBuffSkillTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multipliers, _tableData.multipliers.Length);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.effectMaintainTime);
        _reader.get(_row, ref _tableData.useStatTypes, _tableData.useStatTypes.Length);
        _reader.get(_row, ref _tableData.effectStatTypes, _tableData.effectStatTypes.Length);
        _reader.get(_row, ref _tableData.continuityTypes, _tableData.continuityTypes.Length);
        _reader.get(_row, ref _tableData.particle);
        return true;
    }

    /// <summary>
    /// 플레이어 공격스킬 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerAttackSkill(CSVReader _reader, PlayerAttackSkillTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multiplier);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.effectMaintainTime);
        _reader.get(_row, ref _tableData.effectType);
        _reader.get(_row, ref _tableData.particle);
        return true;
    }
    #endregion
}
