using System;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
public class DialogueReader
{
    #region Load
    public DialogueData LoadDialogueData(string _name)
    {
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueData data = new DialogueData();
        data = JsonUtility.FromJson<DialogueData>(texts);
        return (data == null) ? null : data;
    }

    public List<Dialogue> LoadDialogues(string _name)
    {
        List<Dialogue> dialogues = new List<Dialogue>();
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueData data = new DialogueData();
        data = JsonUtility.FromJson<DialogueData>(texts);
        if (data == null) return null;
        int dataCnt = data.dialogues.Count;
        for (int i = 0; i < dataCnt; i++)
        {
            dialogues.Add(data.dialogues[i]);
        }
        return dialogues;
    }

    public Dictionary<int, Dialogue> LoadDialougeGroup(string _name)
    {
        Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
        DialogueData data = new DialogueData();
        data = JsonUtility.FromJson<DialogueData>(texts);
        if (data == null) return null;
        int dataCnt = data.dialogues.Count;
        for (int i = 0; i < dataCnt; i++)
        {
            dialogueGroup.Add(data.dialogues[i].dialogueID, data.dialogues[i]);
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

    public string ReadChoiceText(string[] _texts, ChoiceSlot _slot)
    {
        if (_texts.Length ==2)
        {
            _slot.SlotEventID = GetEventID(_texts[0]);
            Event(_texts[0], _slot.ChoiceAction);
            return _texts[1];
        }
        else
        {
            _slot.ChoiceAction = null;
            return _texts[0];
        }
    }


    public void Event(string _text)
    {
        int textLen = _text.Length;
        string eventType = string.Empty + _text[0] + _text[1];
        switch (eventType)
        {
            case "$Q":
                AcceptQuest(GetEventID(_text));
                break;
            case "$A":
                AcceptAward(GetEventID(_text));
                break;
        }
    }

    public void Event(string _text, UnityAction<int> _action)
    {
        UnityAction<int> action = null;
        int textLen = _text.Length;
        string eventType = string.Empty + _text[0] + _text[1];
        switch (eventType)
        {
            case "$Q":
                action += AcceptQuest;
                break;
            case "$A":
                action += AcceptAward;
                break;
        }
        _action = action;
    }

    public int GetEventID(string _text)
    {
        return Convert.ToInt32(_text.Substring(2, _text.Length - 2)); 
    }

    public void AcceptQuest(int _id)
    {
        SharedMgr.InteractionMgr.CurrentInteractNPC.AddQuestData
            (SharedMgr.QuestMgr.GetQuestData(_id));
    }

    public void AcceptAward(int _id)
    {

    }
    #endregion
}


/*
대화를 선택해야 퀘스트를 주는 방식
=> 선택지에서 구현
대화만 하면 퀘스트를 주는 방식
 
 
 */
