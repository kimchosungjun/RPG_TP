using SaveDataGroup;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SaveMgr : MonoBehaviour
{
    string directoryPath = string.Empty;

    UserSaveDataReader userDataReader = new UserSaveDataReader();
    InteractioSaveDataReader interactionDataReader = new InteractioSaveDataReader();

    [SerializeField] UserSaveDataGroup userSaveData;
    [SerializeField] InteractionDataGroup interactionData;
    
    public UserSaveDataGroup GetUserSaveData { get { return userSaveData; } }    
    public InteractionDataGroup GetInteractionData { get { return interactionData; } }  

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
        string directoryPath  = GetDirectoryPath();
        userSaveData = userDataReader.GetUserData(directoryPath);
        interactionData = interactionDataReader.GetUserData(directoryPath);
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
        userSaveData = userDataReader.GetUserData();
        interactionData = interactionDataReader.GetInteractionData();
        userDataReader.SaveData(GetDirectoryPath(), userSaveData);
        interactionDataReader.SaveData(GetDirectoryPath(), interactionData);
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
        userSaveData.UpdateAllData();
        yield return null;
        userDataReader.SaveData(GetDirectoryPath(), userSaveData);
    }

    public void ClearUserData() { userSaveData = null; }
    #endregion
}


