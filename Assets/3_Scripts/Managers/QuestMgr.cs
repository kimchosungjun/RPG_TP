using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMgr 
{
    List <QuestSOData> questDatas;
    Dictionary <int,int> questIndexes; // First QuestID, Second List Index
    public List<QuestSOData> GetQuestDatas { get { return questDatas; } }

    public void Init()
    {
        SharedMgr.QuestMgr = this;
        questDatas = new List<QuestSOData>();
        questIndexes = new Dictionary<int, int>();
    }

    public void LoadQuestData()
    {

    }

    public void AddQuestData(QuestSOData _questData)
    {
        if (questIndexes.ContainsKey(_questData.GetQuestID))
            return;

        questDatas.Add(_questData);
        questIndexes.Add(_questData.GetQuestID, questDatas.Count -1);
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }

    public void AddQuestData(int _id)
    {
        string path = "QuestGroup/Quest_SO" + _id;
        QuestSOData data = SharedMgr.ResourceMgr.LoadResource<QuestSOData>(path);

        if (data == null) return;

        if (questIndexes.ContainsKey(_id))
            return;

        questDatas.Add(data);
        questIndexes.Add(data.GetQuestID, questDatas.Count - 1);
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }

    public void DeleteQuestData(int _questID)
    {
        if (questIndexes.ContainsKey(_questID) == false)
            return;

        questDatas.RemoveAt(questIndexes[_questID]);
        questIndexes.Remove(_questID);
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.UpdateQuestDatas();
    }

    public QuestSOData GetQuestData(int _id)
    {
        if (questIndexes.ContainsKey(_id))
            return questDatas[questIndexes[_id]];
        return null;
    }
}
