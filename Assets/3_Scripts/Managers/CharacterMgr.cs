using Photon.Pun;
using UnityEngine;

public partial class ResourceMgr 
{
    public BasePlayer GetBasePlayer(string _playerPrefabName)
    {
        string path = "Players/";
        path += _playerPrefabName;
        return Resources.Load<BasePlayer>(path);
    }

    public GameObject PhotonPlayerInstantiate(string _playerPrefabName, Vector3 _position, Quaternion _rotation)
    {
        string path = "Players/";
        path += _playerPrefabName;
        GameObject result = PhotonNetwork.Instantiate(path, _position, _rotation);
        return result;
    }
}
