using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryObject;
    [SerializeField] InventoryListUI inventoryListUI;
    [SerializeField] InventoryInfoUI inventoryInfoUI;   

    int inventoryCurrentIndex = 0;

    public void ActiveInventory()
    {
        inventoryObject.SetActive(true);
    }

    public void AddItem() { }
    public void RemoveItem() { }
    
    
}


