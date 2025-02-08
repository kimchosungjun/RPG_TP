using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveDataGroup;

[CreateAssetMenu(fileName ="Quest", menuName ="QuestSOData")]
public class QuestSOData : ScriptableObject
{
    #region Value
    [Header("Quest Datas")]
    [SerializeField] int questID;
    [SerializeField] string questName;
    [TextArea(5,10), SerializeField] string questDescription;

    [SerializeField] int notMeetQuestDialogueIndex;
    [SerializeField] int meetQuestDialogueIndex;
    [SerializeField] int afterGetAwardDialogueIndex;

    [Header("Quest Conditions")]
    [SerializeField] List<QuestConditionData> questConditions;
    
    [Header("Quest Awards")]
    [SerializeField] List<ItemAward> itemAwards;
    [SerializeField] List<ExpAward> expAwards;
    [SerializeField] List<CharacterAward> characterAwards;
    
    QuestConditionSaveData saveData;
    public QuestConditionSaveData GetSaveData { get { if (saveData == null) return null; return saveData; } }
    
    public List<QuestConditionData> GetKillConditionData()
    {
        List<QuestConditionData> datas =  new List<QuestConditionData>();
        int cnt = questConditions.Count;
        for(int i=0; i<cnt; i++)
        {
            if(questConditions[i].type ==  QuestEnums.TYPES.KILL)
            {
                datas.Add(questConditions[i]);
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
    public void AcceptQuest()
    {
        saveData = new QuestConditionSaveData(questID, questConditions);
    }

    public bool IsAchieveQuestCondition(ref int _dialogueIndex)
    {
        int conditionCnt = questConditions.Count;
        for (int i = 0; i < conditionCnt; i++)
        {
            if (questConditions[i].IsAchieveQuestCondition() == false)
            {
                _dialogueIndex = notMeetQuestDialogueIndex;
                return false;
            }
        }
        _dialogueIndex = meetQuestDialogueIndex;
        return true;
    }

    public bool CanAchieveQuestAwards()
    {
        //int itemAwardCnt = itemAwards.Count;
        //for(int i=0; i < itemAwardCnt; i++)
        //{
        //    if (itemAwards[i].CanGetAward() == false)
        //        return false;
        //}
        return true;
    }

    public void GetAwards()
    {
        SharedMgr.InteractionMgr.CurrentInteractNPC.DialogueIndex = afterGetAwardDialogueIndex;
        
        if (expAwards.Count != 0)
            expAwards[0].GetAward();

        int itemAwardCnt = itemAwards.Count;
        for (int i = 0; i < itemAwardCnt; i++)
        {
            itemAwards[i].GetAward();
        }

       if(characterAwards.Count !=0)
            characterAwards[0].GetAward();
    }
    #endregion
}