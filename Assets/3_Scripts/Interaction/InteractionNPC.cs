using SaveDataGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    [SerializeField] Animator anim;
    [SerializeField] int dialogueIndex;
    public int GetDialogueIndex { get { return dialogueIndex; } }
    NPCSaveDataGroup saveData = null;
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


    enum NPCState
    {
        Idle=0,
        Talk=1
    }

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
        StartCoroutine(CLookPlayer());
    }

    IEnumerator CLookPlayer()
    {
        Vector3 playerPosition = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position;
        Vector3 direction = playerPosition - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        Quaternion endRot = Quaternion.LookRotation(direction);
        float angle = Vector3.Angle(transform.forward, direction) * 0.5f;

        if (angle <= 30f)
        {
            transform.rotation = endRot;
            anim.SetInteger("State", (int)NPCState.Talk);
            yield break; 
        }

        Quaternion startRot = transform.rotation;
        float time = 0f;

        anim.SetBool("IsTurn", true);
        anim.SetInteger("State", (int)NPCState.Talk);
        while (time < 2f) 
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, time);
            yield return null;
        }
       
    }

    public void AnnounceEndConversation()
    {
        anim.SetBool("IsTurn", false);
        anim.SetInteger("State", (int)NPCState.Idle);
    }

    public void BlockConversation()
    {
        ChangeToDisable();
        SharedMgr.InteractionMgr.RemoveInteractable(this);
    }

}
