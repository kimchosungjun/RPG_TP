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

    [Header("Camera Space")]
    [SerializeField] DashGaugeUI dashGaugeUI;
    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }

    [Header("Overlap")]
    [SerializeField] BossStatusUI bossStatusUI;
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    [SerializeField] ShowGetItemUI showGetItemUI;
    [SerializeField] InteractionUI interactionUI;
    [SerializeField] InventoryUI inventoyUI;
    [SerializeField] QuestUI questUI;
    [SerializeField] DialogueUI dialogueUI;

    public BossStatusUI GetBossStatusUI { get { return bossStatusUI; } }
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    public InventoryUI GetInventoyUI { get { return inventoyUI; } }    
    public ShowGetItemUI GetShowGetItemUI { get { return showGetItemUI; } }
    public InteractionUI GetInteractionUI { get { return interactionUI; } }
    public QuestUI GetQuestUI { get {return questUI;} }
    public DialogueUI GetDialogueUI { get { return  dialogueUI; } } 
    // 나중에 Awake로 변경
    private void Start()
    {
        UILink();
        UIInit();
    }

    public void UILink()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        
        if(dashGaugeUI==null) dashGaugeUI = GetComponentInChildren<DashGaugeUI>();  
        if(bossStatusUI==null) bossStatusUI = GetComponentInChildren<BossStatusUI>();   
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
        if(playerChangeUI==null) playerChangeUI = GetComponentInChildren<PlayerChangeUI>(); 
        if(inventoyUI==null) inventoyUI = GetComponentInChildren<InventoryUI>();    
        if(showGetItemUI==null) showGetItemUI = GetComponentInChildren<ShowGetItemUI>();    
        if(interactionUI==null) interactionUI = GetComponentInChildren<InteractionUI>();
        if(questUI==null) questUI = GetComponentInChildren<QuestUI>();  
        if(dialogueUI ==null) dialogueUI = GetComponentInChildren<DialogueUI>();    
    }

    public void UIInit()
    {
        //interactionUI?.Init();
        bossStatusUI.Init();    
        playerStatusUI.Init();
        playerChangeUI.Init();
        inventoyUI.Init();
        showGetItemUI.Init();
        interactionUI.Init();
        dialogueUI.Init();
        questUI.Init();
    }

    private void Update()
    {
#if UNITY_EDITOR
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
