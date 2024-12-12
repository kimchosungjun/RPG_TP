using System;
using System.Collections.Generic;
using PlayerEnums;

public partial class PlayerTable : BaseTable
{
    /*************************************************************
     ********* 플레이어 데이터 저장 Dictionary **************
    *************************************************************/

    #region Player Data Group : Dictionary 
    // 플레이어의 정보를 저장한 테이블 데이터 : TYPEID가 key 값(id는 enum TYPEID)
    Dictionary<TYPEID, PlayerTableData> playerTableGroup = new Dictionary<TYPEID, PlayerTableData>();

    // 플레이어 레벨 업 정보 (+ 강화 골드)
    PlayerLevelTableData levelupTableData = new PlayerLevelTableData();

    // 누구의 스탯인지 확인하고 레벨에 맞게 스탯을 파싱 : 첫번째 키 값은 플레이어의 TYPEID이고 두번째 키 값은 level
    Dictionary<TYPEID, Dictionary<int, PlayerStatTableData>> playerStatGroup = new Dictionary<TYPEID, Dictionary<int, PlayerStatTableData>>();

    // 누구의 일반공격인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 플레이어 TYPEID, 두번째 키 값은 level
    Dictionary<TYPEID, Dictionary<int, PlayerNormalAttackTableData>> playerNormalAttackDataGroup = new Dictionary<TYPEID, Dictionary<int, PlayerNormalAttackTableData>>();

    // 누구의 버프스킬인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 BUFF_SKILL, 두번째 키 값은 level
    Dictionary<BUFF_SKILL, Dictionary<int, PlayerBuffSkillTableData>> playerBuffSkillDataGroup = new Dictionary<BUFF_SKILL, Dictionary<int, PlayerBuffSkillTableData>>();

    // 누구의 공격스킬인지 확인 후, 레벨에 맞는 공격 데이터 전달 : 첫번째 키 값은 ATTACK_SKILL, 두번째 키 값은 level
    Dictionary<ATTACK_SKILL, Dictionary<int, PlayerAttackSkillTableData>> playerAttackSkillDataGroup = new Dictionary<ATTACK_SKILL, Dictionary<int, PlayerAttackSkillTableData>>();
    #endregion

    /************************************************************
    ********* 플레이어 데이터 반환 Methods ***************
    ************************************************************/

    #region Get Player Data : Key값은 ID (PlayerEnunms.TYPEID, BUFF_SKILL, ATTACK_SKILL) 그리고 Level

    /// <summary>
    /// 플레이어 테이블 정보 반환
    /// </summary>
    /// <param name="_typeID"></param>
    /// <returns></returns>
    public PlayerTableData GetPlayerTableData(TYPEID _typeID)
    {
        if(playerTableGroup.ContainsKey(_typeID))return playerTableGroup[_typeID];
        return null;
    }

    /// <summary>
    /// 플레이어 레벨업 골드, 경험치 정보 반환
    /// </summary>
    /// <returns></returns>
    public PlayerLevelTableData GetPlayerLevelTableData() {  return levelupTableData; }

    /// <summary>
    /// 플레이어 스탯 정보 반환
    /// </summary>
    /// <param name="_typeID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerStatTableData GetPlayerStatTableData(TYPEID _typeID,int _level)
    {
        if (playerStatGroup.ContainsKey(_typeID))
        {
            if (playerStatGroup[_typeID].ContainsKey(_level))
                return playerStatGroup[_typeID][_level];
        }
        return null;
    }

    /// <summary>
    /// 플레이어 기본공격 반환
    /// </summary>
    /// <param name="_typeID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerNormalAttackTableData GetPlayerNormalAttackData(TYPEID _typeID, int _level)
    {
        if (playerNormalAttackDataGroup.ContainsKey(_typeID))
        {
            if (playerNormalAttackDataGroup[_typeID].ContainsKey(_level))
                return playerNormalAttackDataGroup[_typeID][_level];
        }
        return null;
    }

    /// <summary>
    /// 플레이어 버프 스킬 반환
    /// </summary>
    /// <param name="_buffSkillID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerBuffSkillTableData GetPlayerBuffSkillTableData(BUFF_SKILL _buffSkillID, int _level)
    {
        if (playerBuffSkillDataGroup.ContainsKey(_buffSkillID))
        {
            if (playerBuffSkillDataGroup[_buffSkillID].ContainsKey(_level))
                return playerBuffSkillDataGroup[_buffSkillID][_level];
        }
        return null;
    }

    /// <summary>
    /// 플레이어 공격 스킬 반환
    /// </summary>
    /// <param name="_attackSkillID"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public PlayerAttackSkillTableData GetPlayerAttackSkillTableData(ATTACK_SKILL _attackSkillID,int _level)
    {
        if (playerAttackSkillDataGroup.ContainsKey(_attackSkillID)) 
        {
            if (playerAttackSkillDataGroup[_attackSkillID].ContainsKey(_level))
                return playerAttackSkillDataGroup[_attackSkillID][_level];
        }
        return null;
    }

    #endregion


   /************************************************************
   ******** 플레이어 데이터를 저장할 클래스 *************
   ************************************************************/

    #region PlayerDataClasses : 테이블, 레벨, 스탯, 일반공격, 버프스킬, 공격스킬
    [Serializable]
    public class PlayerTableData
    {
        public int id; // enum
        public string name;
        public float speed;
        public float dashSpeed;
        public float jumpSpeed;
        public int normalAttack; // enum
        public int skill; // enum
        public int ultimate; // enum
        public int stat; // enum
    }

    [Serializable]
    public class PlayerLevelTableData
    {
        public int maxLevel;
        public int actionMaxLevel;
        public int[] needExps;
        public int[] normalAttackLevelupGolds;
        public int[] skillLevelupGolds;
        public int[] ultimateLevelupGolds;

        /// <summary>
        /// 최대레벨과 최대행동 레벨을 파싱한 후에 반드시 호출할 것
        /// </summary>
        public void SetSize()
        {
            needExps = new int[maxLevel-1];
            normalAttackLevelupGolds = new int[actionMaxLevel-1];
            skillLevelupGolds = new int[actionMaxLevel-1];
            ultimateLevelupGolds = new int[actionMaxLevel-1];
        }
    }

    [Serializable]
    public class PlayerStatTableData
    {
        public int level;
        public float maxHp;
        public float attackValue;
        public float defenceValue;
        public float criticalValue;
    }

    [Serializable]
    public class PlayerNormalAttackTableData
    {
        public int level;
        public string name;
        public string description;
        public float[] multipliers;
        public int[] effects;
        public int particle; // enum

       public void SetSize(int _combo)
        {
            multipliers = new float[_combo];
            effects = new int[_combo];
        }
    }

    [Serializable]
    public class PlayerBuffSkillTableData
    {
        public int level;
        public string name;
        public string description;
        public float[] multipliers;
        public float coolTime;
        public float effectMaintainTime;
        public int[] useStatTypes; // enum : 사용(계수 비례)하는 스탯
        public int[] effectStatTypes; // enum : 영향을 주는 스탯
        public int[] continuityTypes; // enum : 즉발 여부
        public int particle; // enum
        public void SetSize(int _buffTypeCnt)
        {
            multipliers = new float[_buffTypeCnt];
            useStatTypes = new int[_buffTypeCnt];
            effectStatTypes = new int[_buffTypeCnt];
            continuityTypes = new int[_buffTypeCnt];
        }
    }

    [Serializable]
    public class PlayerAttackSkillTableData
    {
        public int level;
        public string name;
        public string description;
        public float multiplier;
        public float coolTime;
        public float effectMaintainTime;
        public int effectType; // enum : ATTACK_EFFECT_TYPES
        public int particle; // enum
    }
    #endregion
}