using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// c# 바이너리는 쉽게 볼 수 없게 만듬 : 메모장의 경우 풀면 바로 보이는 형태이기에 변환이 불가피한 상황이다.
/// </summary>
public class BaseTable 
{
    string GetTablePath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#else
        //return Application.persistentDataPath + "/Assets";
#endif
    }

    protected void LoadBinary<T>(string _name, ref T _object)
    {
        var binary = new BinaryFormatter();
        binary.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

        TextAsset asset = Resources.Load("Table_" + _name) as TextAsset;
        Stream stream = new MemoryStream(asset.bytes);
        _object = (T)binary.Deserialize(stream);
        stream.Close();
    }

    protected void SaveBinary(string _name, object _object)
    {
        string path = GetTablePath() + "/Table/Resources/" + "Table_" + _name + ".txt";
        var binary = new BinaryFormatter();
        Stream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
        binary.Serialize(stream, _object);
        stream.Close();
    }

    protected CSVReader GetCSVReader(string _name)
    {
        string ext = ".csv";
        FileStream file = new FileStream(".Document/" + _name + ext, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader stream = new StreamReader(file, System.Text.Encoding.UTF8);
        CSVReader reader = new CSVReader();
        reader.parse(stream.ReadToEnd(), false, 1);
        stream.Close();
        return reader;
    }
}
