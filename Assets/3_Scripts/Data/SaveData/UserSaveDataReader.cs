using System.IO;
using UnityEngine;
using SaveDataGroup;

public class UserSaveDataReader
{
    public string GetPath(string _directoryName)
    {
        string dataName = "UserData.json";
        string path = _directoryName+"/"+dataName;
        return path;
    }

    public UserSaveDataGroup GetUserData(string _directoryName)
    {
        string json = File.ReadAllText(GetPath(_directoryName));
        UserSaveDataGroup saveData = JsonUtility.FromJson<UserSaveDataGroup>(json);
        return saveData; 
    }

    public UserSaveDataGroup GetUserData()
    {
        UserSaveDataGroup data = new UserSaveDataGroup();
        return data;
    }

    public void SaveData(string _directoryName ,UserSaveDataGroup _saveData)
    {
        string saveTexts = JsonUtility.ToJson(_saveData);
        File.WriteAllText(GetPath(_directoryName), saveTexts);
    }
}
