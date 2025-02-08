using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class JsonReader 
{
    public abstract bool IsExistDirectory();
    public abstract T GetSaveData<T>(string _name) where T : class;
    public abstract void SaveData<T>(string _path, T _saveData) where T : class;
    public virtual void CreateDirectory(string _directoryName)
    {
        _directoryName = Path.Combine(Application.persistentDataPath, _directoryName);
        if (Directory.Exists(_directoryName))
            return;
        Directory.CreateDirectory(_directoryName);
    }
}
