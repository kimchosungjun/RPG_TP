using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] InventoryUI inventoyUI;
    [SerializeField] DashGaugeUI dashGaugeUI;
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public InventoryUI GetInventoyUI { get { return inventoyUI; } }    
    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }

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
