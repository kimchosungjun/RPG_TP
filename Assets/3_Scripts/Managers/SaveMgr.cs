using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMgr 
{
    SaveDataReader reader = null;
    public void Init()
    {
        SharedMgr.SaveMgr = this;
        reader= new SaveDataReader();
    }

    public void LoadSaveData()
    {

    }
}
