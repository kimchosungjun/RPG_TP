using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 함수를 동기화 할 때 사용
using Photon.Realtime; // 

public class TestPhoton : MonoBehaviourPunCallbacks
{

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        PhotonNetwork.GameVersion = "1.0.0"; // 버전이 같아야지만 같은 게임 가능
        PhotonNetwork.SendRate = 20; // 통신 속도
        PhotonNetwork.SerializationRate = 10; // 1000 = 1초
    }

    private void Start()
    {
        Setup();
    }

    public void Setup() { PhotonNetwork.ConnectUsingSettings(); }

    
}
