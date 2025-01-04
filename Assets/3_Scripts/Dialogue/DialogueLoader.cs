using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader
{
    public DialogueData LoadDialogueData(string _name)
    {
        string path = Application.dataPath + "/Dialogues/" + _name;
        string texts =  SharedMgr.ResourceMgr.LoadResource<TextAsset>(path).text;
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
        int dataCnt =  data.dialogues.Count;
        for(int i=0; i<dataCnt; i++)
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
            dialogueGroup.Add(data.dialogues[i].storyID, data.dialogues[i]);
        }
        return dialogueGroup;
    }
}
