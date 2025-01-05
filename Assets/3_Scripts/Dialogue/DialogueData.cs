using System;
using System.Collections.Generic;

[Serializable]
public class Choice
{
    public int nextID;
    public string choiceText;
}

[Serializable]
public class Dialogue
{
    public int storyID;
    public string storySpeaker;
    public string speakerName;
    public List<string> storyLines;
    public List<Choice> choiceLine;
}

[Serializable]
public class DialogueData
{
    public List<Dialogue> dialogues;
}


