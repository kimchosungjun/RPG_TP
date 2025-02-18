using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime; // 

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    // 콜 백 함수 : 응답이 오면 처리 (기다리는것이 아님)
    public PhotonView PV;

    public void Init() 
    {
        SharedMgr.PhotonMgr = this;
        PhotonNetwork.GameVersion = "1.0.0"; // 버전이 같아야지만 같은 게임 가능
        PhotonNetwork.SendRate = 20; // 통신 속도
        PhotonNetwork.SerializationRate = 10; // 1000 = 1초
    }

    public void Setup()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnConnectedToMaster()
    {
        // 서버에 붙으면 호출
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public void OnLobby()
    {
        PhotonNetwork.IsMessageQueueRunning = true; // 큐 방식으로 처리하겠다.
    }
}
