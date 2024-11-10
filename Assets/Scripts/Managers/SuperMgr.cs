using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMgr : MonoBehaviour
{
    #region Link Manager
    [SerializeField] SceneMgr sceneMgr;
    #endregion
    
    #region Not Link Manager
    HoldItemMgr holdItemMgr = new HoldItemMgr();
    #endregion
    
    private void Awake()
    {
        if (SharedMgr.SuperMgr != null && SharedMgr.SuperMgr != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            SharedMgr.SuperMgr = this;
            sceneMgr.Init();
            holdItemMgr.Init();
        }
    }
}
