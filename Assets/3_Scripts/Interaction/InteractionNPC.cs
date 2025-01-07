using SaveDataGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    [SerializeField] int dialogueIndex;
    NPCSaveData saveData = new NPCSaveData();
    QuestSOData currentQuestData = null;

    #region Manage Quest Data
    public void LoadNpcData()
    {
        
    }

    public void AddQuestData(int _id)
    {
        saveData.currentQuestIndex = _id;
        currentQuestData = SharedMgr.QuestMgr.GetQuestData(_id);
    }
    #endregion

    public void Start()
    {
        SharedMgr.InteractionMgr.LoadDialogue("Girl");
     }

    public override string Detect()
    {
        return "테스트용 NPC 입니다.";
    }

    public override void Interact()
    {
        SharedMgr.InteractionMgr.StartConversation(dialogueIndex);
        SharedMgr.InteractionMgr.RemoveInteractable(this);
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void BlockConversation()
    {
        // index나 layer 변경
    }
}
