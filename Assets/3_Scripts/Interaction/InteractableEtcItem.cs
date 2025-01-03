using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEtcItem : InteractableItem
{
    EtcData etcData = null;
    
    private void Start()
    {
        LoadItemData();
    }

    public override void GetItem()
    {
        SharedMgr.InventoryMgr.AddItem(etcData);    
        Destroy(this.gameObject);
    }

    public override string GetItemName()
    {
        return etcData.itemName;
    }

    public override void LoadItemData()
    {
        etcData = new EtcData();
        etcData.SetData(SharedMgr.TableMgr.GetItem.GetEtcTableData((int)itemID));
    }
}
