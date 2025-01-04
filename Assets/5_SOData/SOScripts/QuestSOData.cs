using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Quest", menuName ="QuestSOData")]
public class QuestSOData : ScriptableObject
{
    #region Value
    [Header("Quest Datas")]
    [SerializeField] int questID;
    [SerializeField] string questName;
    [TextArea(5,10), SerializeField] string questDescription;

    [Header("Quest Conditions")]
    [SerializeField] List<QuestConditionData> questConditions;
    
    [Header("Quest Awards")]
    [SerializeField] List<ItemAward> itemAwards;
    [SerializeField] List<ExpAward> expAwards;

    QuestConditionSaveData saveData;
    public QuestConditionSaveData GetSaveData { get { if (saveData == null) return null; return saveData; } }
    #endregion

    #region Property
    public int GetQuestID {  get { return questID; } }
    public string GetQuestName { get { return questName; } }
    public string GetQuestDescription { get { return questDescription; } }
    #endregion

    #region Achieve Quest
    public void AcceptQuest()
    {
        saveData = new QuestConditionSaveData(questID, questConditions);
    }

    public bool IsAchieveQuestCondition()
    {
        int conditionCnt = questConditions.Count;
        for (int i = 0; i < conditionCnt; i++)
        {
            if (questConditions[i].IsAchieveQuestCondition() == false)
                return false;
        }
        return true;
    }

    public bool CanAchieveQuestAwards()
    {
        int itemAwardCnt = itemAwards.Count;
        for(int i=0; i < itemAwardCnt; i++)
        {
            if (itemAwards[i].CanGetAward() == false)
                return false;
        }
        GetAwards();
        return true;
    }

    public void GetAwards()
    {
        if (expAwards.Count != 0)
            expAwards[0].GetAward();

        int itemAwardCnt = itemAwards.Count;
        for (int i = 0; i < itemAwardCnt; i++)
        {
            itemAwards[i].GetAward();
        }
    }
    #endregion
}
