using ItemTableClassGroup;
using ItemEnums;
using System;

[Serializable]
public class ConsumeData : ItemData
{
    public float multiplier;
    public float maintainTime;
    public int attributeStat;
    public int effectStat;
    public int duration;
    public float defaultValue;
    public int applyStatType;
    public int partyType;

    int maxCnt = 999;
    public int GetMaxCnt { get { return maxCnt; } }

    public override void Remove(int _cnt = 1)
    {
        itemCnt -= _cnt;
        if (itemCnt <= 0)
            SharedMgr.InventoryMgr.RemoveItem(this);
    }

    public override void Use(int _value = 1)
    {
        itemCnt -= _value;
        BasePlayer player = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer;
        TransferConditionData data = new TransferConditionData();
        data.SetData(player.PlayerStat, effectStat, attributeStat, duration, defaultValue, maintainTime, multiplier, applyStatType);
        player.GetPlayerStatControl.AddCondition(data);
        if (itemCnt <= 0)
            SharedMgr.InventoryMgr.RemoveItem(this);
        else
            SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
    }

    public void SetData(ConsumeTableData _tableData, int _cnt = 1)
    {
        itemID = _tableData.ID;
        itemName = _tableData.name;
        itemDescription = _tableData.description;
        itemType = (int)ITEMTYPE.ITEM_ETC;
        itemCnt = _cnt;
        multiplier = _tableData.multiplier;
        maintainTime = _tableData.maintainTime;
        attributeStat = _tableData.attributeStat;
        effectStat = _tableData.effectStat;
        duration    = _tableData.duration;
        defaultValue = _tableData.defaultValue;
        applyStatType = _tableData.applyStatType;
        partyType = _tableData.partyType;
        atlasName = _tableData.atlasName;
        fileName = _tableData.fileName;
        itemIcon = SharedMgr.ResourceMgr.GetSpriteAtlas(atlasName, fileName + "_Icon");
    }
}
