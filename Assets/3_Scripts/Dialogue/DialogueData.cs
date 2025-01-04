using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class DialogueData
{
    public List<Dialogue> dialogues;
}


