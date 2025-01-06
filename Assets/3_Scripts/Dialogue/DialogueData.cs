using System;
using System.Collections.Generic;

[Serializable]
public class Choice
{
    public int nextDialogueID;
    public string[] choiceTexts;
    public bool continueDialouge;
    public int haveQuestID = -1;
}

[Serializable]
public class Dialogue
{
    public int dialogueID;
    public string speakerName;
    public List<string> storyLines;
    public List<Choice> choiceLines;
}

[Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues;
}


