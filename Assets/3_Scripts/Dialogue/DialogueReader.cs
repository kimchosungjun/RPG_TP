using System;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
public class DialogueReader
{
    #region Load
    public DialogueDataSet LoadDialogueData(string _name)
    {
        string path = "Dialogues/" + _name + "Dialogue";
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueDataSet data = new DialogueDataSet();
        data = JsonUtility.FromJson<DialogueDataSet>(texts);
        return (data == null) ? null : data;
    }

    public List<Dialogue> LoadDialogues(string _name)
    {
        List<Dialogue> dialogues = new List<Dialogue>();
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueDataSet data = new DialogueDataSet();
        data = JsonUtility.FromJson<DialogueDataSet>(texts);
        if (data == null) return null;
        int dataCnt = data.dialogueSet.Count;
        for (int i = 0; i < dataCnt; i++)
        {
            dialogues.Add(data.dialogueSet[i]);
        }
        return dialogues;
    }

    public Dictionary<int, Dialogue> LoadDialougeGroup(string _name)
    {
        Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueDataSet data = new DialogueDataSet();
        data = JsonUtility.FromJson<DialogueDataSet>(texts);
        if (data == null) return null;
        int dataCnt = data.dialogueSet.Count;
        for (int i = 0; i < dataCnt; i++)
        {
            dialogueGroup.Add(data.dialogueSet[i].dialogueID, data.dialogueSet[i]);
        }
        return dialogueGroup;
    }
    #endregion

    #region Read
    public string ReadText(string _text, out bool _haveEvent)
    {
        string text = string.Empty;
        switch (_text[0])
        {
            case '$':
                _haveEvent = true;
                Event(_text);
                break;
            default:
                _haveEvent = false;
                text = _text;
                break;
        }
        return text;
    }

    // Dialogue
    public void Event(string _text)
    {
        char eventType = _text[1];
        switch (eventType)
        {
            case 'Q':
                AcceptQuest(GetEventID(_text));
                break;
            case 'A':
                AcceptAward(GetEventID(_text));
                break;
            case 'E':
                EndConversation();
                break;
        }
    }

    public string ReadChoiceText(string[] _texts, ChoiceSlot _slot)
    {
        if (_texts.Length ==2)
        {
              _slot.SlotEventID = GetEventID(_texts[0]);
            Event(_texts[0], _slot);
            return _texts[1];
        }
        else
        {
            _slot.ChoiceAction = null;
            return _texts[0];
        }
    }

    // Choice
    public void Event(string _text, ChoiceSlot _slot)
    {
        UnityAction<int> action = null;
        char eventType = _text[1];
        switch (eventType)
        {
            case 'Q':
                action += AcceptQuest;
                break;
            case 'A':
                action += AcceptAward;
                break;
        }
        _slot.ChoiceAction = action;
    }

    public int GetEventID(string _text)
    {
        return Convert.ToInt32(_text.Substring(2, _text.Length - 2)); 
    }

    public void AcceptQuest(int _id)
    {
        SharedMgr.QuestMgr.AddQuestData(_id);
        SharedMgr.InteractionMgr.CurrentInteractNPC.AddQuestData(_id);
    }

    public void AcceptAward(int _id)
    {

    }

    public void EndConversation()
    {
        SharedMgr.InteractionMgr.CurrentInteractNPC.BlockConversation();
    }
    #endregion
}
