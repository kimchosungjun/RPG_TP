using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterTableClasses;

public partial class MonsterTable : BaseTable
{
    /*************************************************************
    *********** 몬스터 데이터 저장 Dictionary ***************
    *************************************************************/

    #region Player Data Group : Dictionary 
    // 몬스터 ID와 Table Data로 이루어짐
    Dictionary<int, MonsterTableData> monsterTableGroup = new Dictionary<int, MonsterTableData>();
   
    // 몬스터의 StatID의 DropID를 통해 접근 가능
    Dictionary<int, MonsterDropTableData> monsterDropGroup = new Dictionary<int, MonsterDropTableData>();

    // 비전투 몬스터 데이터
    // 첫번째 키 값은 몬스터 ID, 두번째 키 값은 Level 
    Dictionary<int,Dictionary<int,NonCombatMonsterStatTableData>> nonCombatMonsterStatGroup  = new Dictionary<int, Dictionary<int, NonCombatMonsterStatTableData>>();
    #endregion

    /************************************************************
    ********* 플레이어 데이터 반환 Methods ***************
    ************************************************************/

    #region Get Monster Data
   
    public MonsterTableData GetMonsterTableData(int _typeID)
    {
        if (monsterTableGroup.ContainsKey(_typeID)) return monsterTableGroup[_typeID];
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

    #endregion
}
