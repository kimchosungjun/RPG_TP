using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface Item 
{
    public void Use();
    public void Sell();
}

public class ItemData : Item
{
    public int itemID;
    public string itemName;
    public string itemDescription;
    public string itemType;
    public int itemPrice;
    public int itemSellPrice;

    public E_ITEMTYPE ItemType{ get; private set; }

    public virtual void SetItemType()
    {
        Enums.ConvertStringToEnum(itemType, out E_ITEMTYPE result);
        ItemType = result;
    }

    public virtual void Sell()
    {
        // Earn Money : item Sell Price
        
    }

    public virtual void Use()
    {
     
    }
}