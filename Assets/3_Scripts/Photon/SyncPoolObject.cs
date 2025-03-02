using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SyncPoolObject : MonoBehaviour
{
    [SerializeField] PhotonView view;

    private void Awake()
    {
        if(view == null) view = GetComponent<PhotonView>();
    }

    public void DoAnnounceActiveState(bool _isActive)
    {
        view?.RPC("AnnounceActiveState", RpcTarget.All, _isActive);
    }

    [PunRPC]
    public void AnnounceActiveState(bool _isActive) 
    {
        this.gameObject.SetActive(_isActive); 
    }
}
