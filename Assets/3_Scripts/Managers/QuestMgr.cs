using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMgr 
{
    Dictionary <int, QuestConditionDataGroup> killConditions = new Dictionary<int, QuestConditionDataGroup>();
    Dictionary <int ,QuestSOData> questDataGroup= new Dictionary<int, QuestSOData>();
    List <QuestSOData> acceptQuestData = new List<QuestSOData>();
    public List<QuestSOData> GetQuestDatas { get { return acceptQuestData; } }

    #region Load & Set
    public void Init()
    {
        SharedMgr.QuestMgr = this;
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
        questDataGroup.Add(data.GetQuestID, data);
    }
    #endregion

    #region Manage Quest
    public void DeleteQuestData(int _questID)
    {
        int cnt = acceptQuestData.Count;
        for(int i=0; i<cnt; i++)
        {
            if(acceptQuestData[i].GetQuestID == _questID)
            {
                acceptQuestData.RemoveAt(i);
                break;
            }
        }
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }

    public QuestSOData GetQuestData(int _id)
    {
         if(questDataGroup.ContainsKey(_id))
            return questDataGroup[_id];
        return null;
    }

    public void AcceptQuestData(int _questID)
    {
        if (!questDataGroup.ContainsKey(_questID))
            return;

        if (acceptQuestData.Contains(questDataGroup[_questID]))
            return;

        acceptQuestData.Add(questDataGroup[_questID]);
        questDataGroup[_questID].GetKillConditionData();
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