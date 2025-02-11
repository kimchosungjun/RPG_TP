using UnityEngine;
using System;
using System.Collections.Generic;

public interface IAwards 
{
    public void GetAward();
    public Sprite GetAwardSprite();
}

[Serializable]
public class AwardGroup : IAwards
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

    public Sprite GetAwardSprite() { return null;}
}

#region Exp

[Serializable]
public class ExpAward : IAwards
{
    public int awardAmount;

    public void GetAward()
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.
            GetComponent<PartyConditionControl>()?.GetExp(awardAmount);
    }

    public Sprite GetAwardSprite() 
    {
        Sprite result = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_4", "EXP_Icon");
        return result;
    }
}
#endregion

#region Item

[Serializable]
public class ItemAward : IAwards
{
    public ItemEnums.ITEMTYPE itemType;
    public ItemEnums.ITEMID itemID;
    public int itemAmount;
    Sprite iconSprite = null;

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
                iconSprite = etc.GetIcon;
                break;
            case ItemEnums.ITEMTYPE.ITEM_COMSUME:
                ConsumeData consume = new ConsumeData();
                consume.SetData(table.GetConsumeTableData((int)itemID), itemAmount);
                canGetAward = inven.CanAddItem(consume);
                iconSprite = consume.GetIcon;
                break;
            case ItemEnums.ITEMTYPE.ITEM_WEAPON:
                WeaponData weapon = new WeaponData();
                weapon.SetData(table.GetWeaponTableData((int)itemID));
                canGetAward = inven.CanAddItem(weapon);
                iconSprite = weapon.GetIcon;
                break;
            case ItemEnums.ITEMTYPE.ITEM_GOLD:
                canGetAward = true;
                iconSprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_2", "Gold_Icon");
                break;
        }
        return canGetAward;
    }

    public Sprite GetAwardSprite() 
    {
        if(iconSprite == null)
            CanGetAward();
        return iconSprite; 
    }
}
#endregion

#region Character

[Serializable]
public class CharacterAward : IAwards
{
    [SerializeField] PlayerEnums.TYPEIDS playerID;
    [SerializeField] string awardDescription;
    public string GetAwardDescription { get { return awardDescription; } }
    public void GetAward()
    {

    }

    public Sprite GetAwardSprite()
    {
        UnityEngine.Object obj = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_5", "Character_Icon");
        if (obj == null) return null;
        return obj as Sprite;
    }
}

#endregion
