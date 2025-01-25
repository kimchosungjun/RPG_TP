using SaveDataGroup;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SaveMgr : MonoBehaviour
{
    string directoryPath = string.Empty;

    SaveDataReader reader = new SaveDataReader();
    [SerializeField] UserSaveData saveData;

    public UserSaveData GetUserSaveData { get { return saveData; } }    

    public void Init()
    {
        SharedMgr.SaveMgr = this;
    }

    #region Directory

    /// <summary>
    /// Check Directory Exits : (False => Create Directory)
    /// </summary>
    /// <returns></returns>
    public bool IsExistDirectory()
    {
        string persistentPath = Application.persistentDataPath;
        string directoryName = SharedMgr.SceneMgr.GetPlayerID();
        directoryPath =  persistentPath+"/" + directoryName;
        if (!Directory.Exists(directoryPath))
            return false;
        else
            return true;
    }

    public string GetDirectoryPath() 
    {
        if (directoryPath.Equals(string.Empty))
        {
            string persistentPath = Application.persistentDataPath;
            string directoryName = SharedMgr.SceneMgr.GetPlayerID();
            directoryPath = persistentPath +"/"+ directoryName;
            return directoryPath;
        }
        return directoryPath;   
    }
    #endregion

    #region Data (Load & Clear)

    /// <summary>
    /// Action : Open Door UI 
    /// </summary>
    /// <param name="_action"></param>
    public void LoadUserData(UnityAction _action = null) 
    {
        if (IsExistDirectory())
        {
            // exist Save Data
            saveData = reader.GetUserData(GetDirectoryPath());
        }
        else
        {
            // not exist Save Data
            StartCoroutine(CCreateNewData());
        }

        if (_action != null) _action();
    }

    IEnumerator CCreateNewData()
    {
        Directory.CreateDirectory(directoryPath);
        yield return null;
        saveData = reader.GetUserData();
        reader.SaveData(GetDirectoryPath(), saveData);
    }

    #endregion
    public void ClearUserData() { saveData = null; }

    #region Save Data
    public void SaveUserData()  { StartCoroutine(CSaveUserData()); }

    IEnumerator CSaveUserData()
    {
        saveData.UpdateAllData();
        yield return null;
        reader.SaveData(GetDirectoryPath(), saveData);
    }
    #endregion
}


