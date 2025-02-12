using QuestEnums;
using SaveDataGroup;
using System.Collections;
using UnityEngine;

public class InteractionNPC : Interactable
{
    #region Variable
    [SerializeField] bool isDisappearNPC = false;

    bool canInteractable = true;
    string npcName = string.Empty;

    [SerializeField] Animator anim;
    [SerializeField] NPCID npcID;
    QuestSOData currentQuestData = null;

    // Property
    NPCSaveData saveData = null;
    public int DialogueID 
    {
        get 
        {
            if (saveData != null)
                return saveData.saveDialogueID;
            return -1;
        } 
        set
        {
            if(saveData!=null)
                saveData.saveDialogueID = value;
        } 
    }

    public int SetDialogueIndex 
    {
        set 
        {
            if (saveData != null)
                saveData.saveDialogueIndex = value;
            if (value < 0)
                BlockConversation();
        }
    }

    // NPC Local Enum
    enum NPCState
    {
        Idle = 0,
        Talk = 1
    }
    #endregion

    #region Life Cycle
    public void Awake()
    {
        LoadNPCData();
     }

    public void LoadNPCData()
    {
        saveData = SharedMgr.SaveMgr.GetInteractionData.GetNpcSaveData((int)npcID);
        if (saveData == null)
        {
            saveData = new NPCSaveData();
            saveData.npcID = (int)npcID;
            DialogueID = (int)npcID;
            SetDialogueIndex = 0;
            SharedMgr.SaveMgr.GetInteractionData.AddNPCSaveData(saveData);
        }   
    }

    private void Start()
    {
        SetNPCData();
    }

    public void SetNPCData()
    {
        if (saveData == null)
        {
            Debug.Log("세이브 데이터 존재 X");
            return;
        }
        
        npcName = SharedMgr.InteractionMgr.GetDialouge((int)npcID).speakerName;
        if (saveData.saveDialogueID < 0 || saveData.saveDialogueIndex < 0)
        {
            BlockConversation();
            if (isDisappearNPC)
                this.gameObject.SetActive(false);
        }
        if (saveData.npcAcceptQuestID >= 0)
            currentQuestData = SharedMgr.QuestMgr.GetQuestData(saveData.npcAcceptQuestID);
    }

    #endregion

    #region Quest Data

    public void AddQuestData(int _questID)
    {
        if (currentQuestData != null)
            Debug.LogError("Exist Accept Quest!!!!!!");

        currentQuestData = SharedMgr.QuestMgr.GetQuestData(_questID);
        if(currentQuestData!=null)
        {
            if (saveData != null)
                saveData.npcAcceptQuestID = _questID;
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
        if (!canInteractable)
            return;

        if(currentQuestData != null)
        {
            currentQuestData.IsAchieveQuestCondition(ref saveData.saveDialogueIndex);
            SharedMgr.InteractionMgr.StartConversation(this, saveData.saveDialogueIndex);
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
        canInteractable = false;
        ChangeToDisable();
        SharedMgr.InteractionMgr.RemoveInteractable(this);
    }
    #endregion    
}