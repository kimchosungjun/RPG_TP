using System.Collections.Generic;
using UnityEngine;

public class CsvSaver : MonoBehaviour
{
    TableMgr tableMgr;
    [SerializeField] TestSaveData saveData;
    [SerializeField] InClass inclass;
    private void Awake()
    {
        tableMgr = new TableMgr();
        saveData = new TestSaveData();
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
        tableMgr.Player.LoadBinary<TestSaveData>("TestSaveData", ref saveData);
    }

    public void SaveData()
    {
        tableMgr.Player.SaveBinary("TestSaveData", saveData);
    }
}

[System.Serializable]
public class TestSaveData
{
    public int id;
    public int[] lit = new int[2] { 1,2};
    public List<int> list = new List<int>();
    public OverlapData overlap = new OverlapData(); 
}

[System.Serializable]
public class OverlapData
{
    public List<int> overlist = new List<int>();    
    public List<string> overStrList = new List<string>();
}