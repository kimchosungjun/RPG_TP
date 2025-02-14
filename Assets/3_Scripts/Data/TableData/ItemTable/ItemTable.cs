using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemTableClassGroup;

public partial class ItemTable : BaseTable
{
    /*************************************************************
    ************* 플레이어 데이터 저장 Dictionary  **********
    *************************************************************/

    #region Item Data Group : Dictionary 

    // Weapon Upgrade Data
    WeaponUpgradeTableData weaponUpgradeData = new WeaponUpgradeTableData();

    // Etc
    Dictionary<int, EtcTableData> etcDataGroup = new Dictionary<int, EtcTableData>();
    
    // Consume
    Dictionary<int,ConsumeTableData> consumeDataGroup = new Dictionary<int, ConsumeTableData>();
    
    // Weapon
    Dictionary<int, WeaponTableData> weaponDataGroup = new Dictionary<int, WeaponTableData>();

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
    public WeaponUpgradeTableData GetWeaponUpgradeTableData() { return weaponUpgradeData; }

    public EtcTableData GetEtcTableData(int _id)
    {
        if(etcDataGroup.ContainsKey(_id))
            return etcDataGroup[_id];
        return null;
    }

    public ConsumeTableData GetConsumeTableData(int _id)
    {
        if (consumeDataGroup.ContainsKey(_id))
            return consumeDataGroup[_id];
        return null;
    }

    public WeaponTableData GetWeaponTableData(int _id)
    {
        if (weaponDataGroup.ContainsKey(_id))
            return weaponDataGroup[_id];
        return null;
    }

    #endregion
}
