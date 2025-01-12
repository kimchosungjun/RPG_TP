using ItemTableClassGroup;
using ItemEnums;
using System;

[Serializable]
public class ConsumeData : ItemData
{
    public int effect;
    public float effectValue;
    public float maintainTime;

    int maxCnt = 999;
    public int GetMaxCnt { get { return maxCnt; } }

    public override void Remove()
    {
        base.Remove();
    }

    public override void Use()
    {
        base.Use(); 
    }

    public void SetData(ConsumeTableData _tableData, int _cnt = 1)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        itemIcon = null;
        itemTypeIcon = null;
        itemType = (int)ITEMTYPE.ITEM_ETC;
        itemCnt = _cnt;
        effect = _tableData.effect;
        effectValue = _tableData.effectValue;
        maintainTime = _tableData.maintainTime;
    }
}
