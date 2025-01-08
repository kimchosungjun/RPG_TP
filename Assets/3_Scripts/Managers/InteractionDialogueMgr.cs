using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InteractionMgr 
{
    InteractionNPC currentInteractNpc = null;
    public InteractionNPC CurrentInteractNPC { get { return currentInteractNpc; } set { currentInteractNpc = value; } }

    bool isConversation = false;
    public bool GetIsConversation { get { return isConversation; } }

    #region Dialogue Data Value
    Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
    DialogueReader reader = new DialogueReader();
    public DialogueReader GetDialogueReader { get { return reader; } }
    #endregion

    #region Load & Get Dialouge Data
    public void LoadDialogue(string _name)
    {
        DialogueDataSet data = reader.LoadDialogueData(_name);
        int dialogueCnt = data.dialogueSet.Count;
        for (int i = 0; i < dialogueCnt; i++)
        {
            if (dialogueGroup.ContainsKey(data.dialogueSet[i].dialogueID))
                continue;
            dialogueGroup.Add(data.dialogueSet[i].dialogueID, data.dialogueSet[i]);
        }
    }

    public Dialogue GetDialouge(int _dialougeID)
    {
        if (dialogueGroup.ContainsKey(_dialougeID) == false)
            return null;
        return dialogueGroup[_dialougeID];
    }
    #endregion

    #region Conversation

    public void StartConversation(InteractionNPC _npc)
    {
        if (isConversation) 
            return;
        Dialogue data = GetDialouge(_npc.GetDialogueIndex);
        if (data == null)
            return;
        isConversation = true;
        currentInteractNpc = _npc;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.StartConversation(data);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetTalkCamerView(currentInteractNpc.transform);
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.StartConversation(currentInteractNpc.transform.position);
        
        // To Do ~~~
        // InActive UI
    }

    public void ContinueConversation(int _nextDialogueIndex)
    {
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.ContinueConversation(_nextDialogueIndex);
    }

    public void EndConversation()
    {
        isConversation = false;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.EndConversation();
        currentInteractNpc = null;
        SharedMgr.GameCtrlMgr.GetCameraCtrl.ReSetTalkCameraView();
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.EndConversation();
        
        // To Do ~~~
        // Reverse
        //=========
        // InActive UI
    }

    #endregion
}
