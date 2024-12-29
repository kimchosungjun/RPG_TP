using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime;  

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    public void CreateLobbyRoom(string _room)
    {
        if (_room == null)
            return;
        PhotonNetwork.CreateRoom(_room);
    }

    public void RandomLobbyRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinLobbyRoom(string _room)
    {
        if (_room == null)
            return;
        PhotonNetwork.JoinRoom(_room);
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
        foreach(RoomInfo roomInfo in roomList)
        {

        }
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
