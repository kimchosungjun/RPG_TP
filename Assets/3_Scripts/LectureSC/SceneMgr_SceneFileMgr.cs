using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



/// <summary>
/// Txt은 보이는 파일이라 중요한 파일은 저장하면 안됨
/// 옵션같은 파일들을 저장하는 용도
/// </summary>
public partial class SceneMgr : MonoBehaviour
{
    string filePath = "Test.txt";
    public void SaveFile()
    {
        if(File.Exists(Application.dataPath + filePath))
        {
            StreamReader streamReader = File.OpenText(Application.dataPath + filePath);
            string str = streamReader.ReadLine();
            streamReader.Close();
        }
        else
        {
            // 파일은 열었으면 닫아야 됨 : 메모리 누수가 일어난다.
            StreamWriter streamWriter = File.CreateText(filePath);
            streamWriter.WriteLine("Test");
            streamWriter.WriteLine("Test2");
            streamWriter.Close();
        }
    }

    public bool FindFile(string _filePath)
    {
        if (File.Exists(Application.dataPath + _filePath))
            return true;
        return false;
    }

    public void LoadFile(string _filePath)
    {
        if (FindFile(_filePath) == false)
            return;

        // To Do ~~ Load File
    }
}
