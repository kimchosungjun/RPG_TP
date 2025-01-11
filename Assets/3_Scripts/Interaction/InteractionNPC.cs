using SaveDataGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    [SerializeField] int dialogueIndex;
    public int GetDialogueIndex { get { return dialogueIndex; } }
    NPCSaveData saveData = null;
    QuestSOData currentQuestData = null;

    #region Manage Quest Data
    public void LoadNpcData()
    {
        //SharedMgr.SaveMgr.LoadNPCSaveData(this.gameObject.name, ref saveData);
    }

    public void AddQuestData(int _id)
    {
        //saveData.currentQuestIndex = _id;
        currentQuestData = SharedMgr.QuestMgr.GetQuestData(_id);
    }

    #endregion

    public void Start()
    {
        LoadNpcData();
        SharedMgr.InteractionMgr.LoadDialogue(this.gameObject.name);
     }

    public override string Detect()
    {
        return "테스트용 NPC 입니다.";
    }

    public override void Interact()
    {
        SharedMgr.InteractionMgr.StartConversation(this);
        SharedMgr.InteractionMgr.RemoveInteractable(this);
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void BlockConversation()
    {
        ChangeToDisable();
    }

}
