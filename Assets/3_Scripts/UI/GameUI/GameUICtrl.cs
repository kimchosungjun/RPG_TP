using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] InventoryUI inventoyUI;
    [SerializeField] DashGaugeUI dashGaugeUI;
    [SerializeField] InteractionUI interactionUI;
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public InventoryUI GetInventoyUI { get { return inventoyUI; } }    
    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }
    public InteractionUI GetInteractionUI { get { return interactionUI; } }

    private void Start()
    {
        UIInit();
    }

    public void UIInit()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
        if(playerChangeUI==null) playerChangeUI = GetComponentInChildren<PlayerChangeUI>(); 
        if(inventoyUI==null) inventoyUI = GetComponentInChildren<InventoryUI>();    
        if(dashGaugeUI==null) dashGaugeUI = GetComponentInChildren<DashGaugeUI>();  
        if(interactionUI==null) interactionUI = GetComponentInChildren<InteractionUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoyUI.InputInventoryKey();
        }
    }
}
