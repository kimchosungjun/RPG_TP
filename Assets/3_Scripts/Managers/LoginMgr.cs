using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerPrefers Save In Registry


public partial class SceneMgr: MonoBehaviour
{
    #region ID, Password
    public void SetPlayerAccount(string _Key, string _Value)
    {
        PlayerPrefs.SetString(_Key, _Value);
        PlayerPrefs.Save();
    }

    public string GetPlayerAccount(string _ID)
    {
        return PlayerPrefs.GetString(_ID);
    }

    public bool IsExistID(string _ID)
    {
        return (string.Empty == GetPlayerAccount(_ID)) ? false : true;
    }

    public bool IsMaintainLogin() 
    {
        return IsExistID("ID");
    }

    public string GetPlayerID() { return PlayerPrefs.GetString("ID"); }
    #endregion


}
