using UnityEngine;

public class SuperMgr : MonoBehaviour
{
    #region Link Manager
    [SerializeField] SceneMgr sceneMgr;
    [SerializeField] SoundMgr soundMgr;
    #endregion
    
    #region Not Link Manager
    HoldItemMgr holdItemMgr = new HoldItemMgr();
    UIMgr uiMgr = new UIMgr();
    ResourceMgr resourceMgr = new ResourceMgr();
    TableMgr tableMgr = new TableMgr();
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
            resourceMgr.Init();
            uiMgr.Init();
            tableMgr.Init();
            soundMgr.Init();
            // 아이템 정보 : 아직 안씀 
            holdItemMgr.Init();
        }
    }
}
