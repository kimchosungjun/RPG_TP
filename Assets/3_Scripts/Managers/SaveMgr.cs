using SaveDataGroup;
using System.Collections;
using System.IO;
using UnityEngine;
using UtilEnums;
using UnityEngine.Events;

public partial class SaveMgr : MonoBehaviour
{
    #region Variable
    // Read
    string directoryPath;
    SaveJsonDataReader reader;

    // Save Data Group
    UserSaveDataGroup userSaveData = null;
    public UserSaveDataGroup GetUserSaveData { get { return userSaveData; } }

    // Save Data 
    public PlayerSaveDataGroup GetPlayerSaveData { get { return userSaveData.PlayerSaveDataGroup; } }
    public InventorySaveDataGroup GetInventorySaveData { get { return userSaveData.InventorySaveDataGroup; } }
    public InteractionSaveDataGroup GetInteractionData { get { return userSaveData.InteractionSaveDataGroup; } }
    #endregion

    #region Manage Data
    public void Init()
    {
        SharedMgr.SaveMgr = this;
        directoryPath = string.Empty;
        reader = new SaveJsonDataReader();
        LoadOptionFile();
    }

    public void ClearUserData() { userSaveData = null; }
    #endregion

    #region Directory

    public bool IsExistDirectory()
    {
        string persistentPath = Application.persistentDataPath;
        string directoryName = SharedMgr.SceneMgr.GetPlayerID();
        directoryPath = persistentPath + "/" + directoryName;
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
            directoryPath = persistentPath + "/" + directoryName;
            return directoryPath;
        }
        return directoryPath;
    }

    public void CreateDirectory(string _directoryPath) { Directory.CreateDirectory(_directoryPath); }

    #endregion

    #region Load Data 

    /// <summary>
    /// Action : Open Door UI 
    /// </summary>
    /// <param name="_action"></param>
    public void LoadUserData(UnityAction _action = null)
    {
        StopAllCoroutines();
        if (IsExistDirectory())
            StartCoroutine(CLoadExistData(_action));
        else
            StartCoroutine(CCreateNewData(_action));
    }

    IEnumerator CLoadExistData(UnityAction _action)
    {
        // Load Save Data
        userSaveData = new UserSaveDataGroup();
        yield return null;
        userSaveData.PlayerSaveDataGroup = reader.LoadJsonFile<PlayerSaveDataGroup>(GetDirectoryPath(), SAVE_JSON_PATHS.PLAYER);
        userSaveData.InteractionSaveDataGroup = reader.LoadJsonFile<InteractionSaveDataGroup>(GetDirectoryPath(), SAVE_JSON_PATHS.INTERACT);
        userSaveData.InventorySaveDataGroup = reader.LoadJsonFile<InventorySaveDataGroup>(GetDirectoryPath(), SAVE_JSON_PATHS.INVEN);
        yield return null;
        userSaveData.InventorySaveDataGroup.LinkWeaponUniqueIDSet();
        
        SharedMgr.InventoryMgr.LoadInventory(userSaveData.InventorySaveDataGroup);
        // Check Join Lobby 
        while (true)
        {
            if (SharedMgr.PhotonMgr.CheckConnectNetwork() == false)
                yield return null;
            else
                break;
        }
        if (_action != null) _action();
    }

    IEnumerator CCreateNewData(UnityAction _action)
    {
        // Create Directory
        CreateDirectory(directoryPath);
        userSaveData = new UserSaveDataGroup();
        yield return null;
        // Load & Create Save File
        PlayerSaveDataGroup playerSaveDataGroup = new PlayerSaveDataGroup(true);
        InteractionSaveDataGroup interactionSaveDataGroup = new InteractionSaveDataGroup();
        InventorySaveDataGroup inventorySaveDataGroup = new InventorySaveDataGroup();
        userSaveData.PlayerSaveDataGroup = playerSaveDataGroup;
        userSaveData.InteractionSaveDataGroup = interactionSaveDataGroup;
        userSaveData.InventorySaveDataGroup = inventorySaveDataGroup;
        yield return null;
        reader.SaveJsonData<PlayerSaveDataGroup>(playerSaveDataGroup, directoryPath, SAVE_JSON_PATHS.PLAYER);
        reader.SaveJsonData<InteractionSaveDataGroup>(interactionSaveDataGroup, directoryPath, SAVE_JSON_PATHS.INTERACT);
        reader.SaveJsonData<InventorySaveDataGroup>(inventorySaveDataGroup, directoryPath, SAVE_JSON_PATHS.INVEN);
        yield return null;
        // Check Join Lobby 
        while (true)
        {
            if (SharedMgr.PhotonMgr.CheckConnectNetwork() == false)
                yield return null;
            else
                break;
        }
        if (_action != null) _action();
    }

    #endregion

    #region Save Data
    Coroutine saveCoroutine = null;

    public void AutoSave()
    {
        StartCoroutine(CAutoSave());
    }

    IEnumerator CAutoSave()
    {
        yield return new WaitForSeconds(300f);
        if (saveCoroutine != null)
            StartCoroutine(CAutoSave());
        else
            SavePlayerData();
    }

    public void SavePlayerData()
    {
        if (userSaveData == null || saveCoroutine !=null)
            return;
         saveCoroutine = StartCoroutine(CSaveAllData());
    }

    public void SavePlayerData(UnityAction _action)
    {
        if (userSaveData == null || saveCoroutine != null)
            return;
        saveCoroutine = StartCoroutine(CSaveAllData(_action));
    }


    IEnumerator CSaveAllData(UnityAction _action = null)
    {
        userSaveData.UpdateAllData();
        SaveOptionFile();
        yield return null;
        reader.SaveJsonData<PlayerSaveDataGroup>(userSaveData.PlayerSaveDataGroup, directoryPath, SAVE_JSON_PATHS.PLAYER);
        reader.SaveJsonData<InteractionSaveDataGroup>(userSaveData.InteractionSaveDataGroup, directoryPath, SAVE_JSON_PATHS.INTERACT);
        reader.SaveJsonData<InventorySaveDataGroup>(userSaveData.InventorySaveDataGroup, directoryPath, SAVE_JSON_PATHS.INVEN);
        yield return new WaitForSeconds(1f);
        saveCoroutine = null;
        if(_action!=null)
            _action();
    }
    #endregion
}


