using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    public void AddQuestData(QuestSOData _soData)
    {

    }


    private void Awake()
    {
        //SharedMgr.InteractionMgr.LoadDialogue(this.gameObject.name);
        DialogueData data = new DialogueData();

        if(data.dialogues== null)
        {
            Debug.Log("아무것도 없습니다.");
        }
    }

    public override string Detect()
    {
        return "테스트용 NPC 입니다.";
    }

    public override void Interact()
    {
        Debug.Log("성공적인 상호작용 입니다.");
        SharedMgr.InteractionMgr.RemoveInteractable(this);  
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);   
    }
}
