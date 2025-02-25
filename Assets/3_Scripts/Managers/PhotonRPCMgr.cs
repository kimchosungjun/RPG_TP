using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    /***************************/
    /*********  Room *********/
    /***************************/

    #region Room RPC
    [PunRPC]
    public void SendEntryRoom()
    {
        // All, Other, MasterClinet 
        PV.RPC("LobbyRoomEntry", RpcTarget.All);
    }

    [PunRPC]
    public void SendRoomReady()
    {
        PV.RPC("LobbyRoomReady", RpcTarget.All);
    }

    [PunRPC]
    public void SendStartInGame()
    {
        PV.RPC("StartInGame", RpcTarget.All);
    }
    #endregion

    /***************************/
    /*********  Game *********/
    /***************************/
    
    #region Manage SyncObject
    public Dictionary<int, SyncObjectData> syncObjectGroup = new Dictionary<int, SyncObjectData>();

    public void AddSyncObjectData(int _viewID, string _path)
    {
        SyncObjectData syncObject = new SyncObjectData();
        syncObject.photonViewID = _viewID;
        syncObject.resourcePath = _path;

        if (syncObjectGroup.ContainsKey(_viewID))
        {
            Debug.LogError("동일한 View ID 존재!! ");
            return;
        }
        syncObjectGroup.Add(_viewID, syncObject);
    }

    public void UpdateSyncObjectData()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        PhotonView[] views = FindObjectsOfType<PhotonView>();
        int viewCnt = views.Length;
        for (int i = 0; i < viewCnt; i++)
        {
            if (syncObjectGroup.ContainsKey(views[i].ViewID))
                continue;
            syncObjectGroup.Remove(views[i].ViewID);
        }
    }

    public string GetPath(int _viewID)
    {
        string path = string.Empty;
        if (syncObjectGroup.ContainsKey(_viewID))
            path = syncObjectGroup[_viewID].resourcePath;
        return path;
    }
    #endregion

    #region Active State
    public void DoSyncObjectState(int _viewID, bool _isActive, bool _isMasterControl = true)
    {
        PhotonView photonView = PhotonView.Find(_viewID);
        if (photonView == null) return;
        if (_isMasterControl)
        {
            if (photonView.IsMine)
                PV.RPC("SyncObjectState", RpcTarget.All, _viewID, _isActive);
        }
        else
            PV.RPC("SyncObjectState", RpcTarget.All, _viewID, _isActive);
    }

    [PunRPC]
    public void SyncObjectState(int _viewID, bool _isActive)  { PhotonView.Find(_viewID).gameObject?.SetActive(_isActive); }
    #endregion

    #region Transform

    public void DoSyncExistObject(int _viewID, Vector3 _position, Quaternion _rotation)
    {
        photonView.RPC("SyncExistObject", RpcTarget.All, _viewID, _position, _rotation);
    }

    [PunRPC]
    public void SyncExistObject(int _viewID, string _path,Vector3 _position, Quaternion _rotation) 
    {
        if (PhotonView.Find(_viewID) != null)
            return;

    }
    #endregion
}
