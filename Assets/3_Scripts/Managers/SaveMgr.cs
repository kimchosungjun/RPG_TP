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

    public void LoadNPCSaveData(string _npcName, ref NPCSaveDataGroup _saveData)
    {
    }

    public void SaveData<T>(string _savePath) where T : class
    {
        
    }
}
