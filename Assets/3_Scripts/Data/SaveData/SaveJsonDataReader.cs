using System.IO;
using UnityEngine;
using SaveDataGroup;

public class SaveJsonDataReader
{
    public string GetPath(string _directoryPath, UtilEnums.SAVE_JSON_PATHS _jsonPath)
    {
        string fileName = Enums.GetEnumString<UtilEnums.SAVE_JSON_PATHS>(_jsonPath)+".json";
        _directoryPath = Path.Combine(_directoryPath, fileName);
        return _directoryPath;
    }

    public T LoadJsonFile<T>(string _directoryPath, UtilEnums.SAVE_JSON_PATHS _jsonPath) where T : class
    {
        string path = GetPath(_directoryPath, _jsonPath);
        if (!File.Exists(path))
            return null;

        string textAsset = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(textAsset);
        return data;
    }

    public void SaveJsonData<T>(T _data, string _directoryPath, UtilEnums.SAVE_JSON_PATHS _jsonPath) where T : class
    {
        if (_data == null) return;
        string path = GetPath(_directoryPath,_jsonPath);
        string saveTextAsset = JsonUtility.ToJson(_data);
        File.WriteAllText(path, saveTextAsset);
    }
}
