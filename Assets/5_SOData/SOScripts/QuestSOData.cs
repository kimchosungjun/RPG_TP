using System.Collections.Generic;
using UnityEngine;
using SaveDataGroup;

[CreateAssetMenu(fileName ="Quest", menuName ="QuestSOData")]
public class QuestSOData : ScriptableObject
{
    #region Value
    [Header("Quest Datas")]
    bool isClearQuest = false;
    [SerializeField] int questID;
    [SerializeField] string questName;
    [TextArea(5,10), SerializeField] string questDescription;

    [SerializeField] int notMeetQuestDialogueIndex;
    [SerializeField] int meetQuestDialogueIndex;
    [SerializeField] int afterGetAwardDialogueIndex;

    [Header("Quest Conditions")]
    [SerializeField] List<QuestConditionData> questConditionSet;
    public List<QuestConditionData> GetQuestConditionSet { get { return questConditionSet; } }
    
    [Header("Quest Awards")]
    [SerializeField] List<ItemAward> itemAwards;
    [SerializeField] List<ExpAward> expAwards;
    [SerializeField] List<CharacterAward> characterAwards;

    public List<ItemAward> GetItemAwards { get { return itemAwards; } }
    public List<ExpAward > GetExpAwards { get {return expAwards; } }    
    public List<CharacterAward > GetCharacterAwards { get { return characterAwards; } } 
     
    public List<QuestConditionData> GetKillConditionData()
    {
        List<QuestConditionData> datas =  new List<QuestConditionData>();
        int cnt = questConditionSet.Count;
        for(int i=0; i<cnt; i++)
        {
            if(questConditionSet[i].type ==  QuestEnums.TYPES.KILL)
            {
                datas.Add(questConditionSet[i]);
            }
        }
        return datas;
    }
    #endregion

    #region Property
    public int GetQuestID {  get { return questID; } }
    public string GetQuestName { get { return questName; } }
    public string GetQuestDescription { get { return questDescription; } }
    #endregion

    #region Achieve Quest
    public bool IsAchieveQuestCondition(ref int _dialogueIndex)
    {
        int conditionCnt = questConditionSet.Count;
        for (int i = 0; i < conditionCnt; i++)
        {
            if (questConditionSet[i].IsAchieveQuestCondition() == false)
            {
                _dialogueIndex = notMeetQuestDialogueIndex;
                return false;
            }
        }
        _dialogueIndex = meetQuestDialogueIndex;
        UpdateLogData();
        return true;
    }

    public void GetAwards()
    {
        SharedMgr.InteractionMgr.CurrentInteractNPC.SetDialogueIndex = afterGetAwardDialogueIndex;
        
        if (expAwards.Count != 0)
            expAwards[0].GetAward();

        int itemAwardCnt = itemAwards.Count;
        for (int i = 0; i < itemAwardCnt; i++)
        {
            itemAwards[i].GetAward();
        }

       if(characterAwards.Count !=0)
            characterAwards[0].GetAward();

        SharedMgr.QuestMgr.DeleteQuestData(this.questID);
    }
    #endregion

    #region Quest Log Data
    QuestLogData logData = null;

    public void LoadQuestLog()
    {
        logData = new QuestLogData(isClearQuest, questID, questConditionSet);
    }

    public void LoadQuestLog(QuestLogData _logData)
    {
        logData = _logData;
        isClearQuest = _logData.isClearQuest;
        questConditionSet = _logData.conditions;
    }

    public void UpdateLogData()
    {
        if (logData == null)
            logData = new QuestLogData();
        logData.isClearQuest = isClearQuest;
        logData.isClearQuest = isClearQuest;
        logData.conditions = questConditionSet;
    }

    public void UpdateLogData(ref QuestLogData _logData)
    {
        UpdateLogData();
        _logData = logData;
    }
    #endregion
}

