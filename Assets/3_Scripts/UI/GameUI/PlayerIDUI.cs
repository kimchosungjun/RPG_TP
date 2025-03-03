using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIDUI : MonoBehaviour
{
    [SerializeField] Text playerIDText;
    [SerializeField] PhotonView pv;
    public void SetID(string _id)
    {
        if (pv != null)
        {
            DoAnnounceID(_id);
        }
    }

    public void DoAnnounceID(string _id)
    {
        pv.RPC("AnnounceID", RpcTarget.All, _id);
    }

    [PunRPC]
    public void AnnounceID(string _id) 
    {
        playerIDText.text = _id;
    }
}

