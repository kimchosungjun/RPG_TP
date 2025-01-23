using UnityEngine;

public class MgrCreator : MonoBehaviour
{
    #region Reference Link
    [SerializeField] SceneMgr sceneMgr;
    [SerializeField] SoundMgr soundMgr;
    [SerializeField] SaveMgr saveMgr;
    #endregion
    
    #region Use Heap Memory
    UIMgr uiMgr = new UIMgr();
    ResourceMgr resourceMgr = new ResourceMgr();
    TableMgr tableMgr = new TableMgr();
    InventoryMgr inventoryMgr = new InventoryMgr();
    InteractionMgr dialogueMgr = new InteractionMgr();
    QuestMgr questMgr = new QuestMgr(); 
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
            dialogueMgr.Init();
            questMgr.Init();
            saveMgr.Init();
        }
    }
}
