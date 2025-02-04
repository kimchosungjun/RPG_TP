using ItemEnums;
using UnityEngine;

public interface Item 
{
    public void Use(int _value = 1);
    public void Remove(int _cnt = 1);
}

public class ItemData : Item
{
    public int itemID;
    public string itemName;
    public string itemDescription;
    public int itemType;
    public int itemCnt;
    public string fileName;
    public string atlasName;


    protected Sprite itemIcon;
    public Sprite GetIcon { get { return itemIcon; } }

    public virtual void Use(int _value = 1) {  }
    public virtual void Remove(int _cnt = 1) { }

    ITEMTYPE type = ITEMTYPE.ITEM_ETC;
    public virtual bool IsMoney() { if (type == ITEMTYPE.ITEM_GOLD) return true; return false; }

    public ItemData() { }
    public ItemData(ITEMTYPE _itemType, int _itemCnt) { type = _itemType; itemCnt = _itemCnt; }
}