using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime;
using System;
using Unity.VisualScripting; // 

public partial class PhotonMgr : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public void Init()
    {
        SharedMgr.PhotonMgr = this;
        PhotonNetwork.GameVersion = "1.0.0"; 
        PhotonNetwork.SendRate = 20; // Send Speed
        PhotonNetwork.SerializationRate = 10; // 1000 = 1 sec
    }

    public void Setup() { PhotonNetwork.ConnectUsingSettings(); }

    public override void OnDisconnected(DisconnectCause cause) { base.OnDisconnected(cause); }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
}