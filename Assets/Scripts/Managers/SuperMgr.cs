using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMgr : MonoBehaviour
{
    // 씬을 관리하는 매니저
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
            DontDestroyOnLoad(this.gameObject);
            SharedMgr.SuperMgr = this;
            sceneMgr.Init();
        }
    }
}
