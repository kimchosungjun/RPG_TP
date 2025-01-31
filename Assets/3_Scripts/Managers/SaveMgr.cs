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

    #region Load Data (+ Check Join Lobby)

    /// <summary>
    /// Action : Open Door UI 
    /// </summary>
    /// <param name="_action"></param>
    public void LoadUserData(UnityAction _action = null) 
    {
        if (IsExistDirectory())
        {
            // exist Save Data
            StartCoroutine(CLoadExistData(_action));
        }
        else
        {
            // not exist Save Data
            StartCoroutine(CCreateNewData(_action));
        }
    }

    IEnumerator CLoadExistData(UnityAction _action)
    {
        // Load Save Data
        saveData = reader.GetUserData(GetDirectoryPath());
        yield return null;   
        // Check Join Lobby 
        while (true)
        {
            if (SharedMgr.PhotonMgr.CheckConnectLobby() == false)
                yield return null;
            else
                break;
        }
        if (_action != null) _action();
    }

    IEnumerator CCreateNewData(UnityAction _action)
    {
        // Create Directory
        Directory.CreateDirectory(directoryPath);
        yield return null;
        // Load & Create Save File
        saveData = reader.GetUserData();
        reader.SaveData(GetDirectoryPath(), saveData);
        yield return null;
        // Check Join Lobby 
        while (true)
        {
            if (SharedMgr.PhotonMgr.CheckConnectLobby() == false)
                yield return null;
            else
                break;
        }
        if (_action != null) _action();
    }

    #endregion
 
    #region Save & Clear 
    public void SaveUserData()  { StartCoroutine(CSaveUserData()); }

    IEnumerator CSaveUserData()
    {
        saveData.UpdateAllData();
        yield return null;
        reader.SaveData(GetDirectoryPath(), saveData);
    }

    public void ClearUserData() { saveData = null; }
    #endregion
}


