
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Choice 
{
    public bool continueDialouge;
    public int nextDialogueID;
    public string[] choiceTexts;
}

[Serializable]
public class DialogueContent
{
    public int dialogueIndex;
    public List<string> storyLines;
    public List<Choice> choiceLines;
}

[Serializable]
public class Dialogue 
{
    public int questID = -1;
    public int dialogueID;
    public string speakerName;
    public List<DialogueContent> dialogueContentSet;

    #region Get Dialouge Content 

    Dictionary<int, DialogueContent> dialogueGroup = new Dictionary<int, DialogueContent>();
   
    public void InitContents()
    {
        int contentCnt = dialogueContentSet.Count;
        for(int i=0; i<contentCnt; i++)
        {
            if (dialogueGroup.ContainsKey(dialogueContentSet[i].dialogueIndex))
                continue;
            dialogueGroup.Add(dialogueContentSet[i].dialogueIndex, dialogueContentSet[i]);
        }
    }

    public DialogueContent GetContent(int _dialogueIndex)
    {
        if (dialogueGroup.ContainsKey(_dialogueIndex))
            return dialogueGroup[_dialogueIndex];
        return null;
    }

    #endregion
}

[Serializable]
public class DialogueDataSet
{
    public List<Dialogue> dialogueSet;
}


