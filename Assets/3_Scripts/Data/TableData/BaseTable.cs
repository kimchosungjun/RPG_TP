using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// c# 바이너리는 쉽게 볼 수 없게 만듬 : 메모장의 경우 풀면 바로 보이는 형태이기에 변환이 불가피한 상황이다.
/// </summary>
public class BaseTable 
{
    #region GetPath
    /// <summary>
    /// 에디터에선 Assets폴더의 위치 반환, 그 외에는 PersistentDatapath 반환
    /// </summary>
    /// <returns></returns>
    string GetTablePath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#else
        return Application.persistentDataPath + "/Assets";
#endif
    }
    /// <summary>
    /// 폴더 이름 반환
    /// </summary>
    /// <param name="_folderTypes"></param>
    /// <returns></returns>
    string GetFolderPath(UtilEnums.TABLE_FOLDER_TYPES _folderTypes)
    {
        string folderPath = string.Empty;
        switch (_folderTypes)
        {
            case UtilEnums.TABLE_FOLDER_TYPES.NONE:
                break;
            case UtilEnums.TABLE_FOLDER_TYPES.PLAYER:
                folderPath = "PlayerTableCsv/";
                break;
            case UtilEnums.TABLE_FOLDER_TYPES.MONSTER:
                folderPath = "MonsterTableCsv/";
                break;
            case UtilEnums.TABLE_FOLDER_TYPES.ITEM:
                folderPath = "ItemTableCsv/";
                break;
            case UtilEnums.TABLE_FOLDER_TYPES.NPC:
                break;
        }
        return folderPath;
    }
    #endregion

    #region Save & Load
    public void LoadBinary<T>(string _path, ref T _object)
    {
        var binary = new BinaryFormatter();
        binary.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        TextAsset asset = Resources.Load("SaveData/" + _path) as TextAsset;
        Stream stream = new MemoryStream(asset.bytes);
        _object = (T)binary.Deserialize(stream);
        stream.Close();
    }

    public void SaveBinary(string _name, object _object)
    {
        string path = GetTablePath() + "/Resources/SaveData/" +_name + ".txt";
        var binary = new BinaryFormatter();
        Stream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
        binary.Serialize(stream, _object);
        stream.Close();
    }

    /// <summary>
    /// 이름 : 경로 (확장자는 필요 없다. 예시 Player/PlayerTableCsv) 
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public CSVReader GetCSVReader(string _name, UtilEnums.TABLE_FOLDER_TYPES _folderTypes = UtilEnums.TABLE_FOLDER_TYPES.NONE)
    {
        string ext = ".csv";
        FileStream file = new FileStream("Document/" + GetFolderPath(_folderTypes) +_name + ext, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader stream = new StreamReader(file, System.Text.Encoding.UTF8);
        CSVReader reader = new CSVReader();
        reader.parse(stream.ReadToEnd(), false, 1);
        stream.Close();
        return reader;
    }


    #endregion
}
