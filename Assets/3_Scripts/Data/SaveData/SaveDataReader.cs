using System.IO;
using UnityEngine;
using SaveDataGroup;

public class SaveDataReader
{
    public string GetPath(string _directoryName)
    {
        string dataName = "UserData.json";
        string path = Path.Combine(_directoryName, dataName);
        return path;
    }

    public UserSaveData GetUserData(string _directoryName)
    {
        return JsonUtility.FromJson<UserSaveData>(GetPath(_directoryName));
    }

    public UserSaveData GetUserData()
    {
        UserSaveData data = new UserSaveData();
        return data;
    }

    public void SaveData(string _directoryName ,UserSaveData _saveData)
    {
        string saveTexts = JsonUtility.ToJson(_saveData);
        File.WriteAllText(GetPath(_directoryName), saveTexts);
    }
}
