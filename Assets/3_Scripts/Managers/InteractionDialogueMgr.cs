using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InteractionMgr 
{
    InteractionNPC currentInteractNpc = null;
    public InteractionNPC CurrentInteractNPC { get { return currentInteractNpc; } set { currentInteractNpc = value; } }

    bool isConversation = false;

    #region Dialogue Data Value
    Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
    DialogueReader reader = new DialogueReader();
    public DialogueReader GetDialogueReader { get { return reader; } }
    #endregion

    #region Load & Get Dialouge Data
    public void LoadDialogue(string _name)
    {
        DialogueData data = reader.LoadDialogueData(_name);
        int dialogueCnt = data.dialogues.Count;
        for (int i = 0; i < dialogueCnt; i++)
        {
            if (dialogueGroup.ContainsKey(data.dialogues[i].dialogueID))
                continue;
            dialogueGroup.Add(data.dialogues[i].dialogueID, data.dialogues[i]);
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

    public void StartConversation(int _id)
    {
        if (isConversation == false) return;
        Dialogue data = GetDialouge(_id);
        if (data == null) return;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.StartConversation(data);

        // To Do ~~~
        // Camera
        // InActive UI
        // None Control Player
    }

    public void ContinueConversation(int _id)
    {
        Dialogue data = GetDialouge(_id);
        if ( data == null) return;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.ContinueConversation(data);
    }

    public void EndConversation()
    {
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.EndConversation();
        currentInteractNpc = null;

        // To Do ~~~
        // Reverse
        //=========
        // Camera
        // InActive UI
        // None Control Player
    }

    #endregion
}
