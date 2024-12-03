using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestMonsterDataCreate : MonoBehaviour
{
    string pathRootName = string.Empty;
    [SerializeField] string fileName = string.Empty;
    [SerializeField] bool checkFileExist = false;
    [SerializeField] MonsterStat stat;
    private void Awake()
    {
        pathRootName = Application.persistentDataPath;
    }

    private void Start()
    {
        Debug.Log(pathRootName + fileName);

        if (File.Exists(pathRootName + fileName))
        {
            checkFileExist = true;
            MonsterStat loadStat = JsonUtility.FromJson<MonsterStat>(File.ReadAllText(pathRootName + fileName));
            stat = loadStat;
        }
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"데이터 생성"))
        {
            CreateNSave();
        }
    }

    public void CreateNSave()
    {
        if (checkFileExist == false)
        {
            MonsterStat stat = new MonsterStat();
            string texts = JsonUtility.ToJson(stat);
            File.WriteAllText(pathRootName + fileName, texts);
        }
    }
}
