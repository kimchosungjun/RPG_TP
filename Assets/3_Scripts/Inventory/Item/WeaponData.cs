using ItemEnums;

[System.Serializable]
public class WeaponData : ItemData
{
    public int uniqueID;
    public int holdPlayerID = -1;

    // Exp & Level
    public int weaponCurrentExp;
    public int weaponMaxExp;
    public int weaponCurrentLevel;
    public int weaponMaxLevel;
    // Stat
    public float attackValue;
    public float effectValue;
    public int weaponEffect;
    public int weaponType; // staff, bow..
  
    // Enum 값으로 변환 후 저장할 속성
    public WEAPONTYPE WeaponType { get; private set; }
    public WEAPONEFFECT WeaponEffect { get; private set; }
    public bool IsHoldWeapon { get; private set; } = false; // Check For Sell

    public override void Use()
    {
        IsHoldWeapon = true;
    }

    public void Use(int _playerID)
    {
        holdPlayerID = _playerID;
        Use();
    }

    public void TakeOff()
    {
        IsHoldWeapon = false;
        holdPlayerID=-1;
    }

    public void SetData(ItemTableClassGroup.WeaponTableData _tableData)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        itemIcon = null;
        itemTypeIcon = null;
        itemType = (int)ITEMTYPE.ITEM_ETC;
        itemCnt = 1;
        
        attackValue = _tableData.attackValue;
        effectValue = _tableData.additionEffectValue;
        weaponEffect = _tableData.additionalEffect;

        uniqueID = SharedMgr.TableMgr.GetItem.GetWeaponUniqueID();
    }
}
