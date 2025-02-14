using ItemTableClassGroup;
using ItemEnums;
using System;

[Serializable]
public class EtcData : ItemData 
{
    public int etcExp;

    int maxCnt = 999;
    public int GetMaxCnt { get { return maxCnt; } }

    public override void Remove(int _cnt = 1)
    {
        itemCnt -= _cnt;
        SharedMgr.InventoryMgr.AddGold(_cnt * etcExp);
        if (itemCnt <=0)
            SharedMgr.InventoryMgr.RemoveItem(this);
    }

    public override void Use(int _value = 1)
    {
        itemCnt -= _value;
        if (itemCnt <= 0)
            SharedMgr.InventoryMgr.RemoveItem(this);
    }

    public void SetData(EtcTableData _tableData, int _cnt = 1)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        itemType = (int)ITEMTYPE.ITEM_ETC;
        itemCnt = _cnt;
        etcExp = _tableData.exp;
        fileName = _tableData.fileName;
        atlasName = _tableData.atlasName;   
        itemIcon = SharedMgr.ResourceMgr.GetSpriteAtlas(atlasName, fileName + "_Icon");
    }

    public void LoadData(EtcTableData _tableData)
    {
        itemType = (int)ITEMTYPE.ITEM_ETC;
        fileName = _tableData.fileName;
        atlasName = _tableData.atlasName;
        itemIcon = SharedMgr.ResourceMgr.GetSpriteAtlas(atlasName, fileName + "_Icon");
    }
}
