using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BasePlayer : BaseActor
{
    #region Value
    protected bool isAlive = true;
    [SerializeField] int id;
    [Header("플레이어 상태 UI"), SerializeField]protected PlayerStatusUI playerStatusUI; // HP, EXP, Level을 나타내는 UI
    [Header("플레이어 스탯"), SerializeField]protected PlayerStat playerStat; // 확인을 위해 serialize로 설정 : 나중엔 없애기
    [Header("플레이어 행동 관리"), SerializeField] protected PlayerActionControl playerActionControl;
    [Header("플레이어 스탯 관리"), SerializeField] protected PlayerStatControl playerStatControl; // 스탯을 관리(버프도 관리)
    [Header("플레이어 움직임 관리"), SerializeField] protected PlayerMovementControl playerMovementControl;
    #endregion

    #region Property
    public PlayerStat PlayerStat { get { return playerStat; }  set { playerStat = value; } }
    public PlayerStatControl GetPlayerStatControl { get { return playerStatControl; } }
    public PlayerMovementControl GetPlayerMovementControl { get { return playerMovementControl; } }
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }
    
    // Call After Death Animation 
    public void AnnounceDeath()
    {
        PlayerCtrl playerCtrl = GetComponentInParent<PlayerCtrl>();
        playerCtrl?.DeathChangePlayer();
    }

    // Call Immediately : Show Animation
    public void DoDeathState() { playerMovementControl.Death(); isAlive = false; }
    public void DoRevival() { InitState(); isAlive = true; }
    public void InitState() { playerMovementControl.ChangeState(PlayerEnums.STATES.MOVEMENT); }
    public bool GetCanChangeState { get { return playerMovementControl.CanChangePlayer; } }
    #endregion

    /******************************************/
    /************* 레이어 설정  *************/
    /************* 데미지 설정  *************/
    /******************************************/

    #region Override  
    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    public override bool CanTakeDamageState()  { return playerMovementControl.CanTakeDamage; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { playerStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) 
    {
        if (_attackData.GetHitEffect == EffectEnums.HIT_EFFECTS.NONE)
            return;
        playerMovementControl.ApplyTakeDamageState(_attackData.GetHitEffect, _attackData.EffectMaintainTime);
    }
    public override void ApplyCondition(TransferConditionData _conditionData) { playerStatControl.AddCondition(_conditionData);  }

    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Life Cycle : Virtual
    public virtual void Init()
    {
        PlayerSaveStat saveStat;
        // 스탯 연결
        if (id == 0)
            saveStat = new PlayerSaveStat();
        else if(id==1)
            saveStat = new PlayerSaveStat(1,100);
        else
            saveStat = new PlayerSaveStat(2,100);

        playerStat = new PlayerStat();
        playerStat.LoadPlayerStat(saveStat);

        if (playerActionControl == null) playerActionControl = GetComponent<WarriorActionControl>();
        if (playerStatControl == null) playerStatControl = GetComponent<PlayerStatControl>();
        if (playerMovementControl == null) playerMovementControl = GetComponent<WarriorMovementControl>();

        // Link Stat & Player => Check Death State
        playerStatControl.PlayerStat = playerStat;
        playerStatControl.Player = this;
        isAlive = !playerStatControl.CheckDeathState();
        
        playerActionControl.SetPlayerData(playerStatControl, playerMovementControl);
        playerStatusUI = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI;
        playerStatusUI?.Init();

        // 스크립트 연결
       
        playerMovementControl.Init(playerStat);
    }

    public virtual void Setup()
    {
        playerStatusUI.Setup(playerStat);
        playerMovementControl.Setup();
    }

    public virtual void Execute()
    {
        playerStatusUI.FixedExecute();
        playerStatControl.FixedExecute();
        playerMovementControl.Execute();
    }

    public virtual void FixedExecute()
    {
        playerMovementControl.FixedExecute();
    }
    #endregion
}
