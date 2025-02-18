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
    public void DecideObjectState(int _viewID, bool _isActive, bool _isMasterControl = true)
    {
        PhotonView photonView = PhotonView.Find(_viewID);
        if (photonView == null) return;
        if (_isMasterControl)
        {
            if (photonView.IsMine)
                PV.RPC("SyncActiveState", RpcTarget.All, _viewID, _isActive);
        }
        else
            PV.RPC("SyncActiveState", RpcTarget.All, _viewID, _isActive);
    }

    [PunRPC]
    public void SyncActiveState(int _viewID, bool _isActive)  { PhotonView.Find(_viewID).gameObject?.SetActive(_isActive); }
    #endregion

    #region Transform

    public void SetObjectTransform(GameObject _object, Vector3 _position, Quaternion _rotation, bool _isMasterControl = true)
    {
        if (_object == null) return;
        PhotonView photonView = _object.GetComponent<PhotonView>();
        if (photonView == null) return;
        if (_isMasterControl)
        {
            if (photonView.IsMine)
                photonView.RPC("SyncTransform", RpcTarget.All, _object, _position, _rotation);
        }
        else
            photonView.RPC("SyncTransform", RpcTarget.All, _object, _position, _rotation);
    }

    [PunRPC]
    public void SyncTransform(GameObject _object, Vector3 _position, Quaternion _rotation) 
    {
        _object.transform.position = _position;
        _object.transform.rotation = _rotation; 
    }
    #endregion
}
