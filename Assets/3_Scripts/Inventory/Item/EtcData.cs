using ItemTableClasses;
using ItemEnums;

public class EtcData : ItemData 
{
    public int etcExp;

    int maxCnt = 999;
    public int GetMaxCnt { get { return maxCnt; } }

    public override void Remove()
    {
        
    }

    public override void Use()
    {
        
    }

    public void SetData(EtcTableData _tableData, int _cnt = 1)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        itemIcon = null;
        itemTypeIcon = null;
        itemType = (int)ITEMTYPE.ITEM_ETC;
        itemCnt = _cnt;
        etcExp = _tableData.exp;
    }
}
