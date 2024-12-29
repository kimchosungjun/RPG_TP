using System;
using System.Collections.Generic;
using PlayerEnums;
using PlayerTableClasses;

public partial class PlayerTable : BaseTable
{
    /*************************************************************
    ************** 플레이어 데이터 저장 Dictionary **********
    *************************************************************/

    #region Player Data Group : Dictionary 
    // 플레이어의 정보를 저장한 테이블 데이터 : TYPEID가 key 값(id는 enum TYPEID)
    Dictionary<int, PlayerTableData> playerTableGroup = new Dictionary<int, PlayerTableData>();

    // 플레이어 레벨 업 정보 (+ 강화 골드)
    PlayerLevelTableData levelupTableData = new PlayerLevelTableData();

    // 누구의 일반공격인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 플레이어 TYPEID, 두번째 키 값은 level
    Dictionary<int, Dictionary<int, PlayerNormalAttackTableData>> playerNormalAttackDataGroup = new Dictionary<int, Dictionary<int, PlayerNormalAttackTableData>>();

    // 누구의 버프스킬인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 BUFF_SKILL, 두번째 키 값은 level
    Dictionary<int, Dictionary<int, PlayerConditionSkillTableData>> playerBuffSkillDataGroup = new Dictionary<int, Dictionary<int, PlayerConditionSkillTableData>>();

    // 누구의 공격스킬인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 ATTACK_SKILL, 두번째 키 값은 level
    Dictionary<int, Dictionary<int, PlayerAttackSkillTableData>> playerAttackSkillDataGroup = new Dictionary<int, Dictionary<int, PlayerAttackSkillTableData>>();
    #endregion

    /************************************************************
    ************** 플레이어 데이터 반환 Methods **********
    ************************************************************/

    #region Get Player Data : Key값은 ID (PlayerEnunms.TYPEID, BUFF_SKILL, ATTACK_SKILL) 그리고 Level

    /// <summary>
    /// 플레이어 테이블 정보 반환
    /// </summary>
    /// <param name="_typeID"></param>
    /// <returns></returns>
    public PlayerTableData GetPlayerTableData(int _typeID)
    {
        if(playerTableGroup.ContainsKey(_typeID))return playerTableGroup[_typeID];
        return null;
    }

    public PlayerTableData GetPlayerTableData(TYPEIDS _typeID)
    {
        int typeID = (int)_typeID;
        if (playerTableGroup.ContainsKey(typeID)) return playerTableGroup[typeID];
        return null;
    }

    /// <summary>
    /// 플레이어 레벨업 골드, 경험치 정보 반환
    /// </summary>
    /// <returns></returns>
    public PlayerLevelTableData GetPlayerLevelTableData() {  return levelupTableData; }

    /// <summary>
    /// 플레이어 기본공격 반환
    /// </summary>
    /// <param name="_typeID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerNormalAttackTableData GetPlayerNormalAttackData(int _typeID, int _level)
    {
        if (playerNormalAttackDataGroup.ContainsKey(_typeID))
        {
            if (playerNormalAttackDataGroup[_typeID].ContainsKey(_level))
                return playerNormalAttackDataGroup[_typeID][_level];
        }
        return null;
    }

    public PlayerNormalAttackTableData GetPlayerNormalAttackData(TYPEIDS _typeID, int _level)
    {
        int typeID = (int)_typeID;
        if (playerNormalAttackDataGroup.ContainsKey(typeID))
        {
            if (playerNormalAttackDataGroup[typeID].ContainsKey(_level))
                return playerNormalAttackDataGroup[typeID][_level];
        }
        return null;
    }

    /// <summary>
    /// 플레이어 상태 스킬 반환
    /// </summary>
    /// <param name="_buffSkillID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerConditionSkillTableData GetPlayerBuffSkillTableData(int _buffSkillID, int _level)
    {
        if (playerBuffSkillDataGroup.ContainsKey(_buffSkillID))
        {
            if (playerBuffSkillDataGroup[_buffSkillID].ContainsKey(_level))
                return playerBuffSkillDataGroup[_buffSkillID][_level];
        }
        return null;
    }

    public PlayerConditionSkillTableData GetPlayerBuffSkillTableData(CONDITION_SKILLS _buffSkillID, int _level)
    {
        int skillType = (int)_buffSkillID;
        if (playerBuffSkillDataGroup.ContainsKey(skillType))
        {
            if (playerBuffSkillDataGroup[skillType].ContainsKey(_level))
                return playerBuffSkillDataGroup[skillType][_level];
        }
        return null;
    }



    /// <summary>
    /// 플레이어 공격 스킬 반환
    /// </summary>
    /// <param name="_attackSkillID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerAttackSkillTableData GetPlayerAttackSkillTableData(int _attackSkillID, int _level)
    {
        if (playerAttackSkillDataGroup.ContainsKey(_attackSkillID))
        {
            if (playerAttackSkillDataGroup[_attackSkillID].ContainsKey(_level))
                return playerAttackSkillDataGroup[_attackSkillID][_level];
        }
        return null;
    }

    public PlayerAttackSkillTableData GetPlayerAttackSkillTableData(ATTACK_SKILLS _attackSkillID,int _level)
    {
        int skillType = (int)_attackSkillID;
        if (playerAttackSkillDataGroup.ContainsKey(skillType)) 
        {
            if (playerAttackSkillDataGroup[skillType].ContainsKey(_level))
                return playerAttackSkillDataGroup[skillType][_level];
        }
        return null;
    }
    #endregion
}