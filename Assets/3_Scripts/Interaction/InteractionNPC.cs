using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    [SerializeField] int id;
    public void AddQuestData(QuestSOData _soData)
    {

    }

    public void LoadNpcData()
    {

    }

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
        SharedMgr.InteractionMgr.StartConversation(id);
        SharedMgr.InteractionMgr.RemoveInteractable(this);
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void BlockConversation()
    {
        // index나 layer 변경
    }
}
