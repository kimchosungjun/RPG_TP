using Photon.Pun;
using System.IO;
using UnityEngine;

public partial class ResourceMgr 
{
    #region Legacy
    public BasePlayer GetBasePlayer(string _playerPrefabName)
    {
        string path = "Players/";
        path += _playerPrefabName;
        return Resources.Load<BasePlayer>(path);
    }
    #endregion

    #region Photon Instantiate
    public GameObject PhotonPlayerInstantiate(string _playerPrefabName, Vector3 _position, Quaternion _rotation)
    {
        string path = "Players/";
        path += _playerPrefabName;
        GameObject result = PhotonNetwork.Instantiate(path, _position, _rotation);
        return result;
    }

    public Transform PhotonSyncInstantiate(string _path, Vector3 _position, Quaternion _rotation)
    {
        GameObject result = PhotonNetwork.Instantiate(_path, _position, _rotation);
        return result.transform;
    }

    public Transform PhotonSyncInstantiate(string _path)
    {
        GameObject result = PhotonNetwork.Instantiate(_path, Vector3.zero, Quaternion.identity);
        return result.transform;
    }
    #endregion
}