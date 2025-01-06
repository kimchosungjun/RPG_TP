using System;
using System.Collections.Generic;

[Serializable]
public class Choice
{
    public int nextDialogueID;
    public string choiceText;
    public bool continueDialouge;
    public int haveQuestID = -1;
}

[Serializable]
public class Dialogue
{
    public int dialogueID;
    public string speakerName;
    public List<string> storyLines;
    public List<Choice> choiceLine;
}

[Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues;
}


