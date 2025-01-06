using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [Header("Camera Space")]
    [SerializeField] DashGaugeUI dashGaugeUI;

    [Header("Overlap")]
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] InventoryUI inventoyUI;
    [SerializeField] InteractionUI interactionUI;
    [SerializeField] QuestUI questUI;

    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public InventoryUI GetInventoyUI { get { return inventoyUI; } }    
    public InteractionUI GetInteractionUI { get { return interactionUI; } }
    public QuestUI GetQuestUI { get {return questUI;} }
    
    // 나중에 Awake로 변경
    private void Start()
    {
        UILink();
        UIInit();
    }

    public void UILink()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
        if(playerChangeUI==null) playerChangeUI = GetComponentInChildren<PlayerChangeUI>(); 
        if(inventoyUI==null) inventoyUI = GetComponentInChildren<InventoryUI>();    
        if(dashGaugeUI==null) dashGaugeUI = GetComponentInChildren<DashGaugeUI>();  
        if(interactionUI==null) interactionUI = GetComponentInChildren<InteractionUI>();
        if(questUI==null) questUI = GetComponentInChildren<QuestUI>();  
    }

    public void UIInit()
    {
        //interactionUI?.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoyUI.InputInventoryKey();
        }

#if UNITY_EDITOR
        if (interactionUI.IsActive())
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                    interactionUI.Interaction();
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                    interactionUI.InputUpKey();
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                    interactionUI.InputDownKey();
            }
        }
#endif
    }
}
