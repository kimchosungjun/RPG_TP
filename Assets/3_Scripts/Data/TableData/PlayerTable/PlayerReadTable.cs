using System.Collections.Generic;
using PlayerTableClasses;
public partial class PlayerTable: BaseTable
{ 
    /*******************************************************************
    *********** 읽어 온 데이터를 Dictionary에 저장 ***************
    *******************************************************************/

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
    /// 플레이어 기본공격 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerNormalAttackTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerNormalAttackTableData data = new PlayerNormalAttackTableData();
            if (ReadPlayerNormalAttack(reader, data, row, _startCol) == false)
                break;
            if (playerNormalAttackDataGroup.ContainsKey(data.id) == false)
                playerNormalAttackDataGroup[data.id] = new Dictionary<int, PlayerNormalAttackTableData>();

            playerNormalAttackDataGroup[data.id].Add(data.level, data); 
        }
    }

    /// <summary>
    /// 플레이어 상태스킬 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerConditionSkillTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerConditionSkillTableData data = new PlayerConditionSkillTableData();

            if (ReadPlayerConditionSkill(reader, data, row, _startCol) == false)
                break;
            if (playerBuffSkillDataGroup.ContainsKey(data.id) == false)
                playerBuffSkillDataGroup[data.id] = new Dictionary<int, PlayerConditionSkillTableData>();
            playerBuffSkillDataGroup[data.id].Add(data.level, data);
        }
    }

    /// <summary>
    /// 플레이어 공격스킬 데이터 등록
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_startRow"></param>
    /// <param name="_startCol"></param>
    /// <param name="_typeID"></param>
    /// <param name="_comboCnt"></param>
    public void InitPlayerAttackSkillTableCsv(string _name, int _startRow, int _startCol)
    {
        CSVReader reader = GetCSVReader(_name, UtilEnums.TABLE_FOLDER_TYPES.PLAYER);
        for (int row = _startRow; row < reader.row; row++)
        {
            PlayerAttackSkillTableData data = new PlayerAttackSkillTableData();
            if (ReadPlayerAttackSkill(reader, data, row, _startCol) == false)
                break;
            if (playerAttackSkillDataGroup.ContainsKey(data.id) == false)
                playerAttackSkillDataGroup[data.id] = new Dictionary<int, PlayerAttackSkillTableData>();
            playerAttackSkillDataGroup[data.id].Add(data.level, data);
        }
    }
    

    #endregion

    /********************************************************************
    *********** 지정된 클래스의 데이터를 읽어서 파싱 ************
    ********************************************************************/

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
        _reader.get(_row, ref _tableData.defaultHP);
        _reader.get(_row, ref _tableData.defaultAttack);
        _reader.get(_row, ref _tableData.defaultDefence);
        _reader.get(_row, ref _tableData.defaultCritical);
        _reader.get(_row, ref _tableData.defaultAttackSpeed);
        _reader.get(_row, ref _tableData.increaseHP);
        _reader.get(_row, ref _tableData.increaseAttack);
        _reader.get(_row, ref _tableData.increaseDefence);
        _reader.get(_row, ref _tableData.increaseCritical);
        _reader.get(_row, ref _tableData.increaseAttackSpeed);
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
    /// 플레이어 기본공격 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>-
    protected bool ReadPlayerNormalAttack(CSVReader _reader, PlayerNormalAttackTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.id);
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.combo);
        _tableData.SetSize();
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multipliers, _tableData.combo);
        _reader.get(_row, ref _tableData.effectMaintainTimes, _tableData.combo);
        _reader.get(_row, ref _tableData.effects, _tableData.effects.Length);
        _reader.get(_row, ref _tableData.particle);
        return true;
    }

    /// <summary>
    /// 플레이어 상태스킬 데이터 읽기
    /// </summary>
    /// <param name="_reader"></param>
    /// <param name="_tableData"></param>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    protected bool ReadPlayerConditionSkill(CSVReader _reader, PlayerConditionSkillTableData _tableData, int _row, int _col)
    {
        if (_reader.reset_row(_row, _col) == false) return false;
        _reader.get(_row, ref _tableData.id);
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.combo);
        _tableData.SetSize();   
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multipliers, _tableData.combo);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.effectMaintainTime);
        _reader.get(_row, ref _tableData.attributeStatTypes, _tableData.combo);
        _reader.get(_row, ref _tableData.effectStatTypes, _tableData.combo);
        _reader.get(_row, ref _tableData.continuityTypes, _tableData.combo);
        _reader.get(_row, ref _tableData.defaultValues, _tableData.combo);
        _reader.get(_row, ref _tableData.partyType);
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
        _reader.get(_row, ref _tableData.id);
        _reader.get(_row, ref _tableData.level);
        _reader.get(_row, ref _tableData.combo);
        _tableData.SetSize();
        _reader.get(_row, ref _tableData.name);
        _reader.get(_row, ref _tableData.description);
        _reader.get(_row, ref _tableData.multiplier, _tableData.combo);
        _reader.get(_row, ref _tableData.coolTime);
        _reader.get(_row, ref _tableData.effectMaintainTime, _tableData.combo);
        _reader.get(_row, ref _tableData.effectType, _tableData.combo);
        _reader.get(_row, ref _tableData.particle);
        _reader.get(_row, ref _tableData.defaultValues, _tableData.combo);
        return true;
    }
    #endregion
}
