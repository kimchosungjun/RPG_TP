using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMgr 
{
    Dictionary <int, QuestConditionDataGroup> killConditions = new Dictionary<int, QuestConditionDataGroup>();
    List <QuestSOData> questDatas;
    List <QuestSOData> acceptQuestData;
    Dictionary <int,int> questIndexes; // First QuestID, Second List Index
    public List<QuestSOData> GetQuestDatas { get { return acceptQuestData; } }

    #region Load & Set
    public void Init()
    {
        SharedMgr.QuestMgr = this;
        questDatas = new List<QuestSOData>();
        acceptQuestData = new List<QuestSOData>();
        questIndexes = new Dictionary<int, int>();
        LoadQuestData();
    }

    public void LoadQuestData()
    {
        for(int i=0; i<2; i++)
        {
            LoadQuestDataFile(i);
        }
    }

    public void LoadQuestDataFile(int _id)
    {
        string path = "QuestGroup/Quest_SO" + _id;
        QuestSOData data = SharedMgr.ResourceMgr.LoadResource<QuestSOData>(path);
        if (data == null) return;
        questDatas.Add(data);
    }
    #endregion

    #region Manage Quest
    public void DeleteQuestData(int _questID)
    {
        if (questIndexes.ContainsKey(_questID) == false)
            return;

        acceptQuestData.RemoveAt(questIndexes[_questID]);
        questIndexes.Remove(_questID);
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }

    public QuestSOData GetQuestData(int _id)
    {
        if (questIndexes.ContainsKey(_id))
            return questDatas[questIndexes[_id]];
        return null;
    }

    public void AcceptQuestData(int _questID)
    {
        if (questDatas.Count - 1 > _questID)
            return;

        if (acceptQuestData.Contains(questDatas[_questID]))
            return;

        if (questIndexes.ContainsKey(_questID))
            return;

        if (questIndexes.ContainsKey(_questID))
            return;

        acceptQuestData.Add(questDatas[_questID]);
        questIndexes.Add(_questID, acceptQuestData.Count - 1);
        questDatas[_questID].GetKillConditionData();
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }
    #endregion

    #region Kill Quest
    public void KillMonster(int _targetID)
    {
        if (killConditions.ContainsKey(_targetID))
            killConditions[_targetID].KillMonster();
    }

    public void AddKillCondition(QuestConditionData _conditionData)
    {
        int targetID = _conditionData.targetID;
        if (killConditions[targetID] == null )
        {
            QuestConditionDataGroup dataGroup = new QuestConditionDataGroup();  
            dataGroup.AddCondition(_conditionData);
            killConditions.Add(targetID, dataGroup);
            return;
        }
        killConditions[targetID].AddCondition(_conditionData);
    }

    public void AddKillCondition(List<QuestConditionData> _conditions)
    {
        if (_conditions == null)
            return;
        int cnt = _conditions.Count;
        for(int i=0; i< cnt; i++)
        {
            int targetID = _conditions[i].targetID;
            if (killConditions[targetID] == null)
            {
                QuestConditionDataGroup dataGroup = new QuestConditionDataGroup();
                dataGroup.AddCondition(_conditions[i]);
                killConditions.Add(targetID, dataGroup);
                return;
            }
            killConditions[targetID].AddCondition(_conditions[i]);
        }
    }
    #endregion
}


public class QuestConditionDataGroup
{
    public List<QuestConditionData> conditionDataSet = new List<QuestConditionData>();

    public void AddCondition(QuestConditionData _conditionData)
    {
        if (conditionDataSet.Contains(_conditionData))
            return;
        conditionDataSet.Add(_conditionData);
    }

    public void KillMonster()
    {
        int cnt = conditionDataSet.Count;
        for(int i=cnt-1; i>=0; i--)
        {
            conditionDataSet[i].IncreaseTargetCurrentCnt();

            if (conditionDataSet[i].IsAchieveQuestCondition())
                conditionDataSet.RemoveAt(i);   
        }
    }
}