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
