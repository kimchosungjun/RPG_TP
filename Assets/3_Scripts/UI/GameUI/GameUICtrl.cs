using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] InventoryUI inventoyUI;
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public InventoryUI InventoyUI { get { return inventoyUI; } }    
    private void Start()
    {
        UIInit();
    }

    public void UIInit()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
        if(inventoyUI==null) inventoyUI = GetComponentInChildren<InventoryUI>();    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoyUI.InputInventoryKey();
        }
    }
}
