using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryObject;
    
    int inventoryCurrentIndex = 0;

    public void ActiveInventory()
    {
        inventoryObject.SetActive(true);
    }

    public void AddItem() { }
    public void RemoveItem() { }
    
    
}


