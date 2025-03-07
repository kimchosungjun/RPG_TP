using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIDUI : MonoBehaviour
{
    [SerializeField] Text playerIDText;
    [SerializeField] PhotonView pv;
    Transform camTransform = null;

    private void Start()
    {
        camTransform = Camera.main.transform;      
        Canvas canvas = camTransform.GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if(camTransform!=null)
            this.transform.rotation = camTransform.rotation;    
    }

    public void SetID(string _id)
    {
        if (pv != null)
        {
            DoAnnounceID(_id);
        }
    }

    public void DoAnnounceID(string _id)
    {
        pv.RPC("AnnounceID", RpcTarget.AllBuffered, _id);
    }

    [PunRPC]
    public void AnnounceID(string _id) 
    {
        playerIDText.text = _id;
    }
}

