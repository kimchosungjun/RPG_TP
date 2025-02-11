using QuestEnums;
using SaveDataGroup;
using System.Collections;
using UnityEngine;

public class InteractionNPC : Interactable
{
    #region Variable
    [SerializeField] Animator anim;
    [SerializeField] int initDialogueID;
    [SerializeField] int dialogueIndex = 0;
    [SerializeField] NPCID npcID;
    
    public int DialogueID 
    {
        get 
        {
            return initDialogueID; 
        } 
        set
        {
            initDialogueID = value;
            if(saveData!=null)
                saveData.saveDialogueID = value;
            if (value < 0)
                BlockConversation();
        } 
    }
    public int SetDialogueIndex 
    {
        set 
        {
            dialogueIndex = value;
            if (saveData != null)
                saveData.saveDialogueIndex = dialogueIndex;
        }
    }

    string npcName = string.Empty;
    NPCSaveData saveData = null;
    [SerializeField] QuestSOData currentQuestData = null;
    #endregion

    #region NPC Enum (Local)
    enum NPCState
    {
        Idle = 0,
        Talk = 1
    }
    #endregion

    #region Load NPC Data (Life Cycle)
    public void Awake()
    {
        LoadNpcData();
     }

    private void Start()
    {
        npcName = SharedMgr.InteractionMgr.GetDialouge(initDialogueID).speakerName;
    }
    #endregion

    #region Manage Quest Data
    public void LoadNpcData()
    {
        NPCSaveData saveData = SharedMgr.SaveMgr.GetInteractionData.GetNpcSaveData((int)npcID);
        if(saveData != null)
            initDialogueID = saveData.saveDialogueIndex;
        if(saveData == null)
        {
            saveData = new NPCSaveData();
            saveData.npcID = (int)npcID;
            SharedMgr.SaveMgr.GetInteractionData.AddNPCSaveData(saveData);  
        }
    }

    public void AddQuestData(int _id)
    {
        currentQuestData = SharedMgr.QuestMgr.GetQuestData(_id);
        if(currentQuestData!=null)
        {
            if (saveData != null)
                saveData.npcAcceptQuestID = _id;
        }
    }

    public void ClearQuestData()
    {
        currentQuestData = null;
        if(saveData!=null)
            saveData.npcAcceptQuestID = -1;
    }

    #endregion

    #region Interact
    public override string Detect()
    {
        return $"{npcName}와 대화한다.";
    }

    public override void Interact()
    {
        if(currentQuestData != null)
        {
            currentQuestData.IsAchieveQuestCondition(ref dialogueIndex);
            SharedMgr.InteractionMgr.StartConversation(this, dialogueIndex);
        }
        else
        {
            SharedMgr.InteractionMgr.StartConversation(this);
        }
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
    #endregion    
}