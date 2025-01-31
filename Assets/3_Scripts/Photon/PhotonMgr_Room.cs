using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime;
using UnityEngine.Events;

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    protected string serverRoomName = "Ashen";
    List<RoomInfo> curRoomSet;
    const int maxPlayer = 4;

    // Server = Room 
    #region Check Server Info (Exist, PlayerCnt)
    public bool CheckConnectLobby() { return PhotonNetwork.IsConnected ; }

    public bool IsExistServer()
    {
        int cnt = curRoomSet.Count;
        if (cnt == 0)
            return false;
        return true;
    }

    public int GetServerPlayerCnt()
    {
        if (IsExistServer())
            return curRoomSet[0].PlayerCount;
        else
            return 0;
    }

    public bool IsFullRoom(ref bool _isExist)
    {
        if (IsExistServer() == false)
        {
            _isExist = false;
            return false;
        }
        if (GetServerPlayerCnt() < 4)
            return false;
        return true;
    }
    #endregion

    public void CreateLobbyRoom()
    {
        if (IsExistServer()) return;
        RoomOptions option = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = maxPlayer
        };
        PhotonNetwork.JoinOrCreateRoom(serverRoomName, option, null);
    }
     
    public void RandomLobbyRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinLobbyRoom(bool _isExistRoom)
    {
        if (_isExistRoom)
            PhotonNetwork.JoinRoom(serverRoomName);
        else
            CreateLobbyRoom();
    }

    public void LeaveRoom(bool _com)
    {
        PhotonNetwork.LeaveRoom(_com);
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby(); 
    }

    public void SecretLobbyRoom(string _room, byte _secret, byte _maxPlayer)
    {
        if (_room == null)
            return;
        bool open = _secret > 0 ? false : true;
        RoomOptions option = new RoomOptions()
        {
            IsVisible = open,
            MaxPlayers = _maxPlayer
        };

        if (option == null)
            return;

        PhotonNetwork.JoinOrCreateRoom(_room,option,null);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        curRoomSet = roomList;

        if (SharedMgr.SceneMgr.IsLoginScene() == false) return;
        SharedMgr.UIMgr.LoginUICtrl.GetLoginLobbyView.UpdateServerData();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }

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
}
