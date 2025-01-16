using ItemEnums;
using ItemTableClassGroup;
using System;
using System.Diagnostics;

[Serializable]
public class WeaponData : ItemData
{
    public int uniqueID;
    public int holdPlayerID = -1;
    public int weaponPrice;

    // Exp & Level
    public int weaponCurrentExp;
    public int weaponMaxExp;
    public int weaponCurrentLevel = 1;
    public int weaponMaxLevel;
    // Stat
    public int attackValue;
    public float effectValue;
    public float GetEffectValue() { return effectValue * 100; }
    //public int weaponEffect;

    // Enum 값으로 변환 후 저장할 속성
    public WEAPONTYPE WeaponType { get; private set; }
    public WEAPONEFFECT WeaponEffect { get; private set; }
    public bool IsHoldWeapon { get; private set; } = false; // Check For Sell

    public override void Remove(int _cnt = 1)
    {
        SharedMgr.InventoryMgr.AddGold(weaponPrice);
        SharedMgr.InventoryMgr.RemoveItem(this);
    }

    public override void Use(int _value = 1)
    {
        IsHoldWeapon = true;
        holdPlayerID = _value;
        // To Do ~~ 
    }

    public void TakeOff()
    {
        IsHoldWeapon = false;
        holdPlayerID=-1;
        // To Do ~~
    }

    public void SetData(WeaponTableData _tableData)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        weaponPrice = _tableData.price;
        itemType = (int)ITEMTYPE.ITEM_WEAPON;
        itemCnt = 1;
        
        attackValue = _tableData.attackValue;
        effectValue = _tableData.additionEffectValue;
        atlasName = _tableData.atlasName;
        fileName = _tableData.fileName;
        uniqueID = SharedMgr.TableMgr.GetItem.GetWeaponUniqueID();
        itemIcon = SharedMgr.ResourceMgr.GetSpriteAtlas(atlasName, fileName + "_Icon");
        weaponMaxLevel = SharedMgr.TableMgr.GetItem.GetWeaponUpgradeTableData().maxLevel;
        WeaponType = (WEAPONTYPE)_tableData.weaponType;
        WeaponEffect = (WEAPONEFFECT)_tableData.additionalEffect;
        weaponMaxExp = SharedMgr.TableMgr.GetItem.GetWeaponUpgradeTableData().GetNeedExp(weaponCurrentLevel);
        //weaponEffect = _tableData.additionalEffect;
    }

    public void ApplyEnhance(int _exp)
    {
        Tuple<int, int, int, float, int> enhanceResult = PredictEnhance(_exp);
        weaponCurrentLevel = enhanceResult.Item1;
        weaponCurrentExp = enhanceResult.Item2;
        attackValue = enhanceResult.Item3;  
        effectValue = enhanceResult.Item4 / 100f;
        weaponMaxExp = enhanceResult.Item5; 
    }


    /// <summary>
    /// Lv / Exp / Atk / Additional Value / Max Exp
    /// </summary>
    /// <param name="_exp"></param>
    /// <returns></returns>
    public Tuple<int, int, int, float, int> PredictEnhance(int _exp)
    {
        int getExp = _exp;
        int needExp = weaponMaxExp;
        int predictLevel = weaponCurrentLevel;
        int predictExp = weaponCurrentExp;
        while (getExp > 0)
        {
            if (needExp == -1)
                break;

            if ((getExp + predictExp) >= needExp) 
            {
                getExp -= (needExp - predictExp);
                predictLevel += 1;
                needExp = SharedMgr.TableMgr.GetItem.GetWeaponUpgradeTableData().GetNeedExp(predictLevel);
            }
            else
            {
                predictExp = predictExp + getExp;
                break;
            }
        }

        WeaponTableData tableData = SharedMgr.TableMgr.GetItem.GetWeaponTableData(itemID);
        int predictAtkValue = attackValue;
        float predictAdditionValue = effectValue;
        if (tableData != null)
        {
            int diff = predictLevel - weaponCurrentLevel;
            predictAtkValue += tableData.increaseAttackValue * diff;
            predictAdditionValue += tableData.increaseAdditionEffectValue * diff;
            predictAdditionValue *= 100;
        }
        Tuple<int, int, int, float, int> result = new Tuple<int, int, int, float, int>(predictLevel, predictExp, predictAtkValue, predictAdditionValue, needExp);
        return result;
    }

    public string GetAdditionalEffectName()
    {
        string result = string.Empty;
        switch (WeaponEffect)
        {
            case WEAPONEFFECT.WEAPON_ATTACK:
                result = "공격력%";
                break;
            case WEAPONEFFECT.WEAPON_CRITICAL:
                result = "크리티컬 증가";
                break;
        }
        return result;
    }
}
