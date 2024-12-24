using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMgr : MonoBehaviour
{
    [SerializeField] PlayerCtrl playerCtrl;
    public PlayerCtrl GetPlayerCtrl { get { return playerCtrl; } }
    
    private void Awake()
    {
        SharedMgr.EnvironmentMgr = this;
        
        if(playerCtrl==null)
            playerCtrl = FindObjectOfType<PlayerCtrl>();    
    }
}
