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

    public GameObject PhotonRoomTestInstantiate()
    {
        Debug.Log("생성하는중");
        string path = "Players/Cube";
        GameObject result = PhotonNetwork.InstantiateRoomObject(path, new Vector3(67,0,210), Quaternion.identity);
        return result;
    }
}

public struct SyncObjectData
{
    public int photonViewID;
    public string resourcePath;
}