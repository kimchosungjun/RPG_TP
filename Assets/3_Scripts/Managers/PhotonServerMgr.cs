using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime;

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    /***************************/
    /********  Network  *******/
    /***************************/

    public bool CheckConnectNetwork() { return PhotonNetwork.IsConnected ; }

    /***************************/
    /*********  Lobby  ********/
    /***************************/

    #region Lobby
    public bool IsInLobby() { return PhotonNetwork.InLobby; }
    public void JoinLobby() { PhotonNetwork.JoinLobby(); }
    public void LeaveLobby() { PhotonNetwork.LeaveLobby(); }
    #endregion

    /***************************/
    /********  Server   ********/
    /***************************/

    #region Join Server

    const int maxPlayer = 4;
    protected string serverName = "Ashen";

    public void JoinRoom() { PhotonNetwork.JoinRoom(serverName); }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        
        switch (returnCode)
        {
            case ErrorCode.GameDoesNotExist:
                CreateRoomAndJoin();
                break;
            case ErrorCode.GameFull:
                SharedMgr.UIMgr.LoginUICtrl.GetLoginLobbyView.FailJoinServer();
                break;
            default:
                break;
        }
    }

    protected void CreateRoomAndJoin()
    {
        RoomOptions option = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = maxPlayer,
            EmptyRoomTtl = 0
        };
        PhotonNetwork.JoinOrCreateRoom(serverName, option, null);
    }
    public override void OnCreatedRoom() { base.OnCreatedRoom();  }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SharedMgr.SceneMgr.LoadScene(UtilEnums.SCENES.GAME, true);
    }

    #endregion

    #region Left Server
    public void LeaveRoom(bool _com) { PhotonNetwork.LeaveRoom(_com); }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }
        Debug.LogError("Error : Dosen't Exit Room Left");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SharedMgr.SceneMgr.LoadScene(UtilEnums.SCENES.LOGIN, true);
    }

    #endregion
}
