using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMgr : MonoBehaviour
{
    Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
    DialogueLoader loader;
    
    public void Init()
    {
        loader = new DialogueLoader();
        SharedMgr.DialogueMgr = this;
    }

    // Call By NPC
    public void LoadDialogue(string _name)
    {
        DialogueData data = loader.LoadDialogueData(name);
        int dialogueCnt = data.dialogues.Count;
        for(int i = 0; i < dialogueCnt; i++)
        {
            if (dialogueGroup.ContainsKey(data.dialogues[i].storyID))
                continue;
            dialogueGroup.Add(data.dialogues[i].storyID, data.dialogues[i]);    
        }
    }

    public Dialogue GetDialouge(int _dialougeID)
    {
        if (dialogueGroup.ContainsKey(_dialougeID) == false)
            return null;
        return dialogueGroup[_dialougeID];
    }
}
