using UIEnums;
using UnityEngine;


public class GameUICtrl : MonoBehaviour
{
    bool isConversationState = false;
    public GAMEUI CurrentOpenUI { get; set; } = GAMEUI.NONE;

    public bool CanAccessUI()
    {
        return !isConversationState;
    }

    [Header("Raw Render : Model UI")]
    [SerializeField] UIModelCam modelCam;
    public UIModelCam GetModelCam { get { return modelCam; } }

    [Header("Camera Space")]
    [SerializeField] DashGaugeUI dashGaugeUI;
    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }

    [Header("Overlap")]
    [SerializeField] SubBossStatusUI bossStatusUI;
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] ShowGetItemUI showGetItemUI;
    [SerializeField] InteractionUI interactionUI;
    [SerializeField] PlayerPartyUI playerPartyUI;
    [SerializeField] UpgradeIndicatorUI upgradeIndicatorUI;
    [SerializeField] InventoryUI inventoyUI;
    [SerializeField] QuestUI questUI;
    [SerializeField] DialogueUI dialogueUI;

    public SubBossStatusUI GetBossStatusUI { get { return bossStatusUI; } }
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public ShowGetItemUI GetShowGetItemUI { get { return showGetItemUI; } }
    public InteractionUI GetInteractionUI { get { return interactionUI; } }
    public PlayerPartyUI GetPlayerPartyUI { get { return playerPartyUI; } } 
    public UpgradeIndicatorUI GetUpgradeIndicatorUI { get { return upgradeIndicatorUI; } }
    public InventoryUI GetInventoyUI { get { return inventoyUI; } }    
    public QuestUI GetQuestUI { get {return questUI;} }
    public DialogueUI GetDialogueUI { get { return  dialogueUI; } } 
    // 나중에 Awake로 변경
    private void Awake()
    {
        UILink();
        UIInit();
        SharedMgr.CursorMgr.SetCursorVisibleState(false);
    }

    public void UILink()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        // Model UI
        if(modelCam==null) modelCam =FindObjectOfType<UIModelCam>();    
        // Camera Space
        if(dashGaugeUI==null) dashGaugeUI = GetComponentInChildren<DashGaugeUI>();  
        // Overlay
        if(bossStatusUI==null) bossStatusUI = GetComponentInChildren<SubBossStatusUI>();   
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
        if(playerChangeUI==null) playerChangeUI = GetComponentInChildren<PlayerChangeUI>(); 
        if(showGetItemUI==null) showGetItemUI = GetComponentInChildren<ShowGetItemUI>();    
        if(interactionUI==null) interactionUI = GetComponentInChildren<InteractionUI>();
        if(playerPartyUI==null) playerPartyUI = GetComponentInChildren<PlayerPartyUI>();
        if (upgradeIndicatorUI==null) upgradeIndicatorUI = GetComponentInChildren<UpgradeIndicatorUI>(); 
        if(inventoyUI==null) inventoyUI = GetComponentInChildren<InventoryUI>();    
        if(questUI==null) questUI = GetComponentInChildren<QuestUI>();  
        if(dialogueUI ==null) dialogueUI = GetComponentInChildren<DialogueUI>();    
    }

    public void UIInit()
    {
        // Model 
        modelCam.Init();
        // Camera Space
        dashGaugeUI.Init();
        // Overlay
        bossStatusUI.Init();    
        playerStatusUI.Init();
        playerChangeUI.Init();
        showGetItemUI.Init();
        interactionUI.Init();
        playerPartyUI.Init();
        upgradeIndicatorUI.Init();
        inventoyUI.Init();
        questUI.Init();
        dialogueUI.Init();
    }

    private void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        // Cursor
        if(CurrentOpenUI == GAMEUI.NONE)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                SharedMgr.CursorMgr.SetCursorVisibleState(true);
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
                SharedMgr.CursorMgr.SetCursorVisibleState(false);
        }

        if (isConversationState)
        {
            if (dialogueUI.GetIsChoiceActive)
            {
                float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
                if (Input.GetKeyDown(KeyCode.F) && CanOpenUI(GAMEUI.QUEST))
                {
                    dialogueUI.SelectChoice();
                }
                if (scroll > 0f)
                {
                    dialogueUI.InputUpKey();
                }
                if (scroll < 0f)
                {
                    dialogueUI.InputDownKey();
                }
            }

            if (SharedMgr.InteractionMgr.GetIsConversation && Input.GetKeyDown(KeyCode.Space))
            {
                dialogueUI.InputNext();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.I) && CanOpenUI(GAMEUI.INVENTORY))
            {
                inventoyUI.InputInventoryKey();
            }

            if (Input.GetKeyDown(KeyCode.J) && CanOpenUI(GAMEUI.QUEST))
            {
                questUI.InputQuestKey();
            }

            if (Input.GetKeyDown(KeyCode.P) && CanOpenUI(GAMEUI.PLAYER_PARTY))
            {
                playerPartyUI.InputPartyKey();
            }

            if (interactionUI.CanInput() && CanOpenUI(GAMEUI.INTERACT))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactionUI.Interaction();
                }

                float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
                if (scroll > 0f)
                {
                    interactionUI.InputUpKey();
                }
                if (scroll < 0f)
                {
                    interactionUI.InputDownKey();
                }
            }
        }
#endif
    }

    public void StartConversation()
    {
        isConversationState = true;
        playerStatusUI.TurnOff();
        playerChangeUI.TurnOff();
        showGetItemUI.TurnOff();
        interactionUI.TurnOff();
    }

    public void EndConversation()
    {
        isConversationState = false;
        playerStatusUI.TurnOn();
        playerChangeUI.TurnOn();
        showGetItemUI.TurnOn();
        interactionUI.TurnOn();
    }

    public bool CanOpenUI(GAMEUI _uiType)
    {
        if (CurrentOpenUI == GAMEUI.NONE)
            return true;

        if (_uiType == CurrentOpenUI)
            return true;

        return false;
    }
}
