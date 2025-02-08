using SaveDataGroup;
using System.IO;
using UnityEngine;

public class InteractioSaveDataReader 
{
    public string GetPath(string _directoryName)
    {
        string dataName = "QuestData.json";
        string path = _directoryName + "/" + dataName;
        return path;
    }

    public InteractionDataGroup GetUserData(string _directoryName)
    {
        string json = File.ReadAllText(GetPath(_directoryName));
        InteractionDataGroup saveData = JsonUtility.FromJson<InteractionDataGroup>(json);
        return saveData;
    }

    public InteractionDataGroup GetInteractionData()
    {
        InteractionDataGroup data = new InteractionDataGroup();
        return data;
    }

    public void SaveData(string _directoryName, InteractionDataGroup _saveData)
    {
        string saveTexts = JsonUtility.ToJson(_saveData);
        File.WriteAllText(GetPath(_directoryName), saveTexts);
    }
}


