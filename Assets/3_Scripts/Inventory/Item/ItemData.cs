using UnityEngine;

public interface Item 
{
    public void Use();
    public void Remove();
}

public class ItemData : Item
{
    public int itemID;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public Sprite itemTypeIcon;
    public int itemType;
    public int itemCnt;

    public virtual void Use() {  }

    public virtual void Remove() { }
}