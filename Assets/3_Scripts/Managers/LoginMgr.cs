using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerPrefers는 레지스트리에 저장한다. 


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

    /// <summary>
    /// 존재하면 True, 없다면 False
    /// </summary>
    /// <param name="_ID"></param>
    /// <returns></returns>
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
