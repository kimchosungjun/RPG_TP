using System.Collections.Generic;
using UIEnums;
using UnityEngine;


public class GameUICtrl : MonoBehaviour
{
    #region Variable
    bool isConversationState = false;
    private GAMEUI gameUI = GAMEUI.NONE;
    public GAMEUI CurrentOpenUI 
    {
        get
        {
            return gameUI;
        }
        set
        {
            gameUI = value;
            SetCurrentUI(value);
        }
    } 

    List<IInputKeyUI> inputKeyUISet = new List<IInputKeyUI>();  

    public void SetCurrentUI(GAMEUI _curUI)
    {
        if(_curUI == GAMEUI.NONE)
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
            PlayUISFX(true);
        }
        else
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true);
            PlayUISFX(false);
        }
    }
    #endregion

    #region InputKeyUI Enum (Local Enum)

    public enum InputKeyUITypes
    {
        Quest=0,
        Inven,
        Party,
        Setting,
        Exit
    }

    #endregion

    [Header("Raw Render : Model UI")]
    [SerializeField] UIModelCam modelCam;
    public UIModelCam GetModelCam { get { return modelCam; } }

    [Header("Camera Space")]
    [SerializeField] DashGaugeUI dashGaugeUI;
    public DashGaugeUI GetDashGaugeUI { get { return dashGaugeUI; } }


    [Header("Overlap")]
    [SerializeField] IndicatorUI indicatorUI;
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
    [SerializeField] SettingUI settingUI;
    [SerializeField] GameExitUI gameExitUI;

    public IndicatorUI GetIndicatorUI { get { return indicatorUI; } }
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
    public SettingUI GetSettingUI { get { return settingUI; } } 
    public GameExitUI GameExitUI { get {  return gameExitUI; } }

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
        if(indicatorUI==null) indicatorUI= GetComponentInChildren<IndicatorUI>();   
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
        if(settingUI==null) settingUI = GetComponentInChildren<SettingUI>();
        if(gameExitUI ==null) gameExitUI = GetComponentInChildren<GameExitUI>(); 

        if(inputKeyUISet.Count!=0)
            inputKeyUISet.Clear();

        inputKeyUISet.Add(questUI);
        inputKeyUISet.Add(inventoyUI);
        inputKeyUISet.Add(playerPartyUI);
        inputKeyUISet.Add(settingUI);
        inputKeyUISet.Add(gameExitUI);
    }

    public void UIInit()
    {
        // Model 
        modelCam.Init();
        // Camera Space
        dashGaugeUI.Init();
        // Overlay
        indicatorUI.Init();
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
        settingUI.Init();
        gameExitUI.Init();
    }

    private void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        // Cursor
        if(CanOpenUI(gameUI) && isConversationState == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true); 
                SharedMgr.CursorMgr.SetCursorVisibleState(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);   
                SharedMgr.CursorMgr.SetCursorVisibleState(false);
            }
        }

        if (isConversationState)
        {
            if (dialogueUI.GetIsChoiceActive)
            {
                float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
                if (Input.GetKeyDown(KeyCode.F))
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
            // Input Key
            if (Input.GetKeyDown(KeyCode.I) && CanOpenUI(GAMEUI.INVENTORY))
            {
                inputKeyUISet[(int)InputKeyUITypes.Inven].InputKey();
            }
            if (Input.GetKeyDown(KeyCode.J) && CanOpenUI(GAMEUI.QUEST))
            {
                inputKeyUISet[(int)InputKeyUITypes.Quest].InputKey();
            }
            if (Input.GetKeyDown(KeyCode.P) && CanOpenUI(GAMEUI.PLAYER_PARTY))
            {
                inputKeyUISet[(int)InputKeyUITypes.Party].InputKey();
            }
            if (Input.GetKeyDown(KeyCode.O) && CanOpenUI(GAMEUI.SETTING))
            {
                inputKeyUISet[(int)InputKeyUITypes.Setting].InputKey();
            }
            if (Input.GetKeyDown(KeyCode.Escape) && CanOpenUI(GAMEUI.EXIT))
            {
                inputKeyUISet[(int)InputKeyUITypes.Exit].InputKey();
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

    public void InputUIIcon(InputKeyUITypes _type)
    {
        inputKeyUISet[(int)_type].InputKey();
    }

    public bool CanAccessUI() { return !isConversationState; }

    public void StartConversation()
    {
        isConversationState = true;

        indicatorUI.TurnOff();  
        playerStatusUI.TurnOff();
        playerChangeUI.TurnOff();
        showGetItemUI.TurnOff();
        interactionUI.TurnOff();

        SharedMgr.CursorMgr.SetCursorVisibleState(true);
    }

    public void EndConversation()
    {
        isConversationState = false;
        
        indicatorUI.TurnOn();
        playerStatusUI.TurnOn();
        playerChangeUI.TurnOn();
        showGetItemUI.TurnOn();
        interactionUI.TurnOn();

        SharedMgr.CursorMgr.SetCursorVisibleState(false);
    }

    public bool CanOpenUI(GAMEUI _uiType)
    {
        if (CurrentOpenUI == GAMEUI.NONE)
            return SharedMgr.GameCtrlMgr.GetPlayerCtrl.CanInteractUI();

        if (_uiType == CurrentOpenUI)
            return true;

        return false;
    }

    public void PlayUISFX(bool _isNone)
    {
        if (_isNone)
            SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.INVEN_CLOSE_SFX);
        else
            SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.INVEN_OPEN_SFX);
    }

    public bool CanControlPlayer()
    {
        if (isConversationState || CurrentOpenUI != GAMEUI.NONE)
            return false;
        return true;
    }
}
