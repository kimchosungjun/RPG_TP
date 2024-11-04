using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMgr : MonoBehaviour
{
    [SerializeField] SceneMgr sceneMgr;
    private void Awake()
    {
        if (SharedMgr.SuperMgr != null && SharedMgr.SuperMgr != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            SharedMgr.SuperMgr = this;
            sceneMgr.Init();
        }
    }
}
