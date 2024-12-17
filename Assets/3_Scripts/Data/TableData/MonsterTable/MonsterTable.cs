using System.Collections.Generic;
using MonsterTableClasses;
using MonsterEnums;

public partial class MonsterTable : BaseTable
{
    /********************************************************
    *********** 몬스터 데이터 저장 Dictionary **********
    ********************************************************/

    #region Monster Data Group : Dictionary 

    // 몬스터 ID와 Table Data로 이루어짐
    Dictionary<int, MonsterInfoTableData> monsterInfoTableGroup = new Dictionary<int, MonsterInfoTableData>();
   
    // 몬스터의 StatID의 DropID를 통해 접근 가능
    Dictionary<int, MonsterDropTableData> monsterDropGroup = new Dictionary<int, MonsterDropTableData>();

    // 비전투 몬스터 데이터
    // 첫번째 키 값은 몬스터 ID, 두번째 키 값은 Level 
    Dictionary<int,Dictionary<int,NonCombatMonsterStatTableData>> nonCombatMonsterStatGroup  = new Dictionary<int, Dictionary<int, NonCombatMonsterStatTableData>>();

    // 전투 몬스터 데이터
    // 첫번째 키 값은 몬스터 ID, 두번째 키 값은 Level
    Dictionary<int, Dictionary<int, CombatMonsterStatTableData>> combatMonsterStatGroup = new Dictionary<int, Dictionary<int, CombatMonsterStatTableData>>();

    #endregion

    /*****************************************************
    ********* 플레이어 데이터 반환 Methods ********
    *****************************************************/

    #region Get Monster Data
    
    public MonsterInfoTableData GetMonsterInfoTableData(int _typeID)
    {
        if (monsterInfoTableGroup.ContainsKey(_typeID)) return monsterInfoTableGroup[_typeID];
        return null;
    }

    public MonsterInfoTableData GetMonsterInfoTableData(TYPEIDS _typeID)
    {
        int typeID = (int)_typeID;
        if (monsterInfoTableGroup.ContainsKey(typeID)) return monsterInfoTableGroup[typeID];
        return null;
    }

    public MonsterDropTableData GetMonsterDropTableData(int _keyID) 
    {
        if(monsterDropGroup.ContainsKey(_keyID))
            return monsterDropGroup[_keyID];
        return null;
    }

    public NonCombatMonsterStatTableData GetNonCombatMonsterStatTableData(int _typeID, int _level)
    {
        if (nonCombatMonsterStatGroup.ContainsKey(_typeID))
        {
            if (nonCombatMonsterStatGroup[_typeID].ContainsKey(_level))
            {
                return nonCombatMonsterStatGroup[_typeID][_level];
            }
        }
        return null;
    }
    
    public NonCombatMonsterStatTableData GetNonCombatMonsterStatTableData(TYPEIDS _typeID, int _level)
    {
        int typeID = (int)_typeID;
        if (nonCombatMonsterStatGroup.ContainsKey(typeID))
        {
            if (nonCombatMonsterStatGroup[typeID].ContainsKey(_level))
            {
                return nonCombatMonsterStatGroup[typeID][_level];
            }
        }
        return null;
    }

    public CombatMonsterStatTableData GetCombatMonsterStatTableData(int _typeID, int _level)
    {
        if (combatMonsterStatGroup.ContainsKey(_typeID))
        {
            if (combatMonsterStatGroup[_typeID].ContainsKey(_level))
            {
                return combatMonsterStatGroup[_typeID][_level];
            }
        }
        return null;
    }

    public CombatMonsterStatTableData GetCombatMonsterStatTableData(TYPEIDS _typeID, int _level)
    {
        int typeID = (int)_typeID;
        if (combatMonsterStatGroup.ContainsKey(typeID))
        {
            if (combatMonsterStatGroup[typeID].ContainsKey(_level))
            {
                return combatMonsterStatGroup[typeID][_level];
            }
        }
        return null;
    }
   
    #endregion
}
