using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class testSaver : MonoBehaviour
{
    TableMgr tableMgr = new TableMgr();
    SaveDataGroup.UserSaveData datas;
    private void OnGUI()
    {
        
        if(GUI.Button(new Rect(0, 0, 100, 100), "Save"))
        {
            SaveDataGroup.UserSaveData data = new SaveDataGroup.UserSaveData();
            //tableMgr.GetPlayer.SaveBinary("save", data);
            string text = JsonUtility.ToJson(data);
            File.WriteAllText(Application.dataPath + "/Save.json", text);
        }

        if (GUI.Button(new Rect(200, 0, 100, 100), "Load"))
        {

            tableMgr.GetPlayer.LoadBinary<SaveDataGroup.UserSaveData>("save", ref datas);
        }
    }
}
