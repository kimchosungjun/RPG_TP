using UnityEngine;

public class SuperMgr : MonoBehaviour
{
    #region Reference Link
    [SerializeField] SceneMgr sceneMgr;
    [SerializeField] SoundMgr soundMgr;
    #endregion
    
    #region Use Heap Memory
    HoldItemMgr holdItemMgr = new HoldItemMgr();
    UIMgr uiMgr = new UIMgr();
    ResourceMgr resourceMgr = new ResourceMgr();
    TableMgr tableMgr = new TableMgr();
    InventoryMgr inventoryMgr = new InventoryMgr();
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
            tableMgr.Init();
            sceneMgr.Init();
            resourceMgr.Init();
            uiMgr.Init();
            soundMgr.Init();
            inventoryMgr.Init();
            // 아이템 정보 : 아직 안씀 
            holdItemMgr.Init();
        }
    }
}
