using UnityEngine;
using System;
using System.Collections.Generic;

public interface IQuestAwards 
{
    public void GetAward();
}

[Serializable]
public class AwardGroup : IQuestAwards
{
    [SerializeField] List<ExpAward> expAwards;
    [SerializeField] List<ItemAward> itemAwards;
    [SerializeField] List<CharacterAward> characterAwards;
    
    public void GetAward()
    {
        int expCnt = expAwards.Count;
        int itemCnt = itemAwards.Count; 
        int characterCnt = characterAwards.Count;   

        // Call Sequence : 1. Exp, 2 : Charater
        if(expCnt!=0)
            expAwards[0].GetAward();    
        if(characterCnt !=0)
            characterAwards[0].GetAward();
        for(int i=0; i<itemCnt; i++)
        {
            itemAwards[i].GetAward();
        }
    }
}

#region Exp

[Serializable]
public class ExpAward : IQuestAwards
{
    public int awardAmount;

    public void GetAward()
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.
            GetComponent<PartyConditionControl>()?.GetExp(awardAmount);
    }
}
#endregion

#region Item

[Serializable]
public class ItemAward : IQuestAwards
{
    public ItemEnums.ITEMTYPE itemType;
    public ItemEnums.ITEMID itemID;
    public int itemAmount;

    public void GetAward()
    {
        ItemTable table = SharedMgr.TableMgr.GetItem;
        InventoryMgr inven = SharedMgr.InventoryMgr;
        switch (itemType)
        {
            case ItemEnums.ITEMTYPE.ITEM_ETC:
                EtcData etc = new EtcData();
                etc.SetData(table.GetEtcTableData((int)itemID), itemAmount);
                inven.AddItem(etc);
                break;
            case ItemEnums.ITEMTYPE.ITEM_COMSUME:
                ConsumeData consume = new ConsumeData();
                consume.SetData(table.GetConsumeTableData((int)itemID), itemAmount);
                inven.AddItem(consume);
                break;
            case ItemEnums.ITEMTYPE.ITEM_WEAPON:
                WeaponData weapon= new WeaponData();
                weapon.SetData(table.GetWeaponTableData((int)itemID));
                inven.AddItem(weapon);
                break;
            case ItemEnums.ITEMTYPE.ITEM_GOLD:
                inven.AddGold(itemAmount);
                break;
        }
    }

    public bool CanGetAward()
    {
        ItemTable table = SharedMgr.TableMgr.GetItem;
        InventoryMgr inven = SharedMgr.InventoryMgr;
        bool canGetAward = false;
        switch (itemType)
        {
            case ItemEnums.ITEMTYPE.ITEM_ETC:
                EtcData etc = new EtcData();
                etc.SetData(table.GetEtcTableData((int)itemID), itemAmount);
                canGetAward = inven.CanAddItem(etc);
                break;
            case ItemEnums.ITEMTYPE.ITEM_COMSUME:
                ConsumeData consume = new ConsumeData();
                consume.SetData(table.GetConsumeTableData((int)itemID), itemAmount);
                canGetAward = inven.CanAddItem(consume);
                break;
            case ItemEnums.ITEMTYPE.ITEM_WEAPON:
                WeaponData weapon = new WeaponData();
                weapon.SetData(table.GetWeaponTableData((int)itemID));
                canGetAward = inven.CanAddItem(weapon);
                break;
            case ItemEnums.ITEMTYPE.ITEM_GOLD:
                canGetAward = true;
                break;
        }
        return canGetAward;
    }
}
#endregion

#region Character

[Serializable]
public class CharacterAward : IQuestAwards
{
    [SerializeField] PlayerEnums.TYPEIDS playerID;
    public void GetAward()
    {

    }
}

#endregion
