using SaveDataGroup;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveMgr 
{
    SaveDataReader reader = null;
    public void Init()
    {
        SharedMgr.SaveMgr = this;
        reader= new SaveDataReader();
    }

    public void LoadNPCSaveData(string _npcName, ref NPCSaveData _saveData)
    {
        string path = Application.persistentDataPath + $"/SaveData/{_npcName}+Dialogue";
        if (File.Exists(path) == false)
        { 

        }
        NPCSaveData saveData =new NPCSaveData();
        Debug.Log(path);    
        //string loatText = SharedMgr.ResourceMgr.LoadResource<TextAsset>(path)?.text;
        //NPCSaveData saveData = JsonUtility.FromJson<NPCSaveData>(loatText);
        _saveData = saveData;
    }

    public void SaveData<T>(string _savePath) where T : class
    {
        
    }
}
