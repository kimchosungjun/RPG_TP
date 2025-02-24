using System.Collections.Generic;
using UIEnums;
using UnityEngine;

public class GameUICtrl : MonoBehaviour
{
    UIBaseControl uiBaseControl = new UIBaseControl();
    public UIBaseControl GetUIBaseControl { get { return uiBaseControl; } }

    #region InputKeyUI Enum

    public enum InputKeyUITypes
    {
        Quest=0,
        Inven,
        Party,
        Setting,
        Exit,
        Save
    }

    #endregion

    #region Link UI : Private
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
    [SerializeField] EtcUI etcUI;
    #endregion

    #region UI Property
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
    public GameExitUI GetGameExitUI { get {  return gameExitUI; } }
    public EtcUI GetEtcUI { get {   return etcUI; } }
    #endregion

    /*****************************/
    /********* AWAKE **********/
    /*****************************/
    
    #region Init
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

        uiBaseControl.AddUIBase(questUI);
        uiBaseControl.AddUIBase(inventoyUI);
        uiBaseControl.AddUIBase(playerPartyUI);
        uiBaseControl.AddUIBase(settingUI);
        uiBaseControl.AddUIBase(gameExitUI);
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
    #endregion

    /*****************************/
    /******** UPDATE *********/
    /*****************************/

    #region Manage Input 
    private void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        InputCursor();
        InputConversation();
        InputUIKey();
#endif
    }

    void InputCursor()
    {
        if (uiBaseControl.IsOpenUI() == false && isConversationState == false)
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
    }

    void InputConversation()
    {
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
    }

    void InputUIKey()
    {
        if (isConversationState) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            uiBaseControl.GetUIBase(InputKeyUITypes.Inven).InputKey();
            ManageControl();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            uiBaseControl.GetUIBase(InputKeyUITypes.Quest).InputKey();
            ManageControl();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            uiBaseControl.GetUIBase(InputKeyUITypes.Party).InputKey();
            ManageControl();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            uiBaseControl.GetUIBase(InputKeyUITypes.Setting).InputKey();
            ManageControl();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiBaseControl.IsOpenUI() == false)
                uiBaseControl.GetUIBase(InputKeyUITypes.Exit).InputKey();
            else
                uiBaseControl.PeekUIPopup()?.InputKey();
            ManageControl();
        }

        // Interaction
        if (interactionUI.CanInput() && !uiBaseControl.IsOpenUI())
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

    public void ManageControl()
    {
        if (uiBaseControl.IsHaveActiveUI())
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(true);
            SharedMgr.CursorMgr.SetCursorVisibleState(true);
        }
        else
        {
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetPlayerControl(false);
            SharedMgr.CursorMgr.SetCursorVisibleState(false);
        }
    }

    public void InputUIIcon(InputKeyUITypes _type) { uiBaseControl.GetUIBase(_type).InputKey(); }
    #endregion

    #region Relate Coversation

    bool isConversationState = false;
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
    public bool CanControlPlayer()
    {
        if (isConversationState || uiBaseControl.IsOpenUI())
            return false;
        return true;
    }
    #endregion

    public void PlayUISFX(bool _isNone)
    {
        if (_isNone)
            SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.INVEN_CLOSE_SFX);
        else
            SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.INVEN_OPEN_SFX);
    }
}
