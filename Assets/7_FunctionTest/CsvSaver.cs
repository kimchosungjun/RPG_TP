using System.Collections.Generic;
using UnityEngine;

public class CsvSaver : MonoBehaviour
{
    TableMgr tableMgr;
    [SerializeField] PlayerSaveStat saveData;
    [SerializeField] InClass inclass;
    private void Awake()
    {
        tableMgr = new TableMgr();
        //saveData = new PlayerSaveStat();
    }
    [System.Serializable]
    public class InClass
    {
        public int id;
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "Save"))
        {
            SaveData();
        }

        if (GUI.Button(new Rect(150, 0, 100, 100), "Load"))
        {
            LoadData();
        }
    }

    public void LoadData()
    {
        tableMgr.GetPlayer.LoadBinary<PlayerSaveStat>("TestSaveData", ref saveData);
    }

    public void SaveData()
    {
        tableMgr.GetPlayer.SaveBinary("TestSaveData", saveData);
    }
}