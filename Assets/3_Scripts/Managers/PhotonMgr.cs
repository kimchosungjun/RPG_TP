using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime;
using System; // 

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

    public void Setup() { PhotonNetwork.ConnectUsingSettings(); }

    public override void OnDisconnected(DisconnectCause cause) { base.OnDisconnected(cause); }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() { base.OnJoinedLobby(); }

    /// <summary>
    /// Manage Queue
    /// </summary>
    public void OnLobby() { PhotonNetwork.IsMessageQueueRunning = true; }


    #region Test Function
    public void CreateSyncObject()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        TDG tdg = new TDG();
        PV.RPC("CreateRPC", RpcTarget.All, tdg.path, tdg.position, tdg.rotation);
    }

    [PunRPC]
    public void CreateRPC(string path, Vector3 position, Quaternion rotation)
    {
        Instantiate(SharedMgr.ResourceMgr.LoadResource<Transform>(path).gameObject, position, rotation);
    }
    #endregion
}


[Serializable]
public class TDG
{
    public string path = "Cube";
    public Vector3 position = new Vector3(162, 0, 210);
    public Quaternion rotation = Quaternion.identity;
}