using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerPrefers는 레지스트리에 저장한다. 


public partial class SceneMgr: MonoBehaviour
{
    #region String형 값 저장 & 반환
    public void SetPlayerPrefersIntKey(string _key, string _valueType)
    {
        PlayerPrefs.SetString(_key, _valueType);
        PlayerPrefs.Save();
    }

    public string GetPlayerPrefersStringKey(string _key)
    {
        return PlayerPrefs.GetString(_key);
    }
    #endregion


}
