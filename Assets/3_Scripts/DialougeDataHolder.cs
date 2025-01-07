using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialougeDataHolder : MonoBehaviour
{
    [Header("대사 생성 데이터")]
    [SerializeField] DialogueDataSet dialougeData;
    [SerializeField] string dialogueName;
    string dialogueFileType = ".json";

    public void CreateData()
    {
        string dataPath = Application.dataPath + "/" + dialogueName + dialogueFileType;
        string textFile = JsonUtility.ToJson(dialougeData);
        if (File.Exists(dataPath))
        {
            Debug.Log(dataPath);
            Debug.LogError("이미 존재하는 파일 이름!!");
            return;
        }
        File.WriteAllText(dataPath, textFile);
    }
}
