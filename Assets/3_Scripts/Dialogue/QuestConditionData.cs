using System;
using System.Collections.Generic;
using QuestEnums;
using UnityEngine;

[Serializable]
public class QuestConditionData
{
    string description = string.Empty;
    public TYPES type;
    public string targetName;
    public int targetID;
    public int targetCnt;
    public int currentCnt;

    public void IncreaseTargetCurrentCnt()
    {
        if (currentCnt >= targetCnt)
            return;
        currentCnt += 1;
    }

    public bool IsAchieveQuestCondition()
    {
        bool isAchieveQuest = true;
        switch (type)
        {
            case TYPES.ITEM:
                ItemData itemData = SharedMgr.InventoryMgr.GetItemData(targetID);
                currentCnt = (itemData == null) ? 0 : itemData.itemCnt;
                isAchieveQuest = (targetCnt <= currentCnt) ? true : false;
                break;
            case TYPES.KILL:
                isAchieveQuest = (currentCnt >= targetCnt) ? true : false;
                break;
            case TYPES.HELP:
                isAchieveQuest = (currentCnt >= targetCnt) ? true : false;
                break;
        }
        return isAchieveQuest;
    }

    /// <summary>
    /// ref => Change Indicate Description Color
    /// </summary>
    /// <param name="_isClear"></param>
    /// <returns></returns>
    public string GetQuestDescription(ref bool _isClear)
    {
        switch (type)
        {
            case TYPES.ITEM:
                ItemData itemData = SharedMgr.InventoryMgr.GetItemData(targetID);
                currentCnt = (itemData == null) ? 0 : itemData.itemCnt;
                _isClear = (targetCnt <= currentCnt) ? true : false;
                description = targetName + $"{currentCnt}/{targetCnt}";
                break;
            case TYPES.KILL:
                _isClear = (currentCnt >= targetCnt) ? true : false;
                description = targetName + $"처치 {currentCnt}/{targetCnt}";  
                break;
            case TYPES.HELP:
                _isClear = (currentCnt >= targetCnt) ? true : false;
                description = targetName + $"처치 {currentCnt}/{targetCnt}";
                break;
        }

        return string.Empty;
    }
}

[Serializable]
public class QuestConditionSaveData
{
    [SerializeField] int questID; 
    [SerializeField] List<QuestConditionData> conditions = new List<QuestConditionData>();

    public QuestConditionSaveData() { }
    public QuestConditionSaveData(int _questID, List<QuestConditionData> _datas)
    {
        questID = _questID;
        conditions = _datas;
    }
}