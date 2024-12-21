using UnityEngine;

public class BasePlayer : BaseActor
{
    protected bool isAlive = true;
    [SerializeField] int id;
    [Header("플레이어 상태 UI"), SerializeField]protected PlayerStatusUI playerStatusUICtrl; // HP, EXP, Level을 나타내는 UI
    [Header("플레이어 스탯"), SerializeField]protected PlayerStat playerStat; // 확인을 위해 serialize로 설정 : 나중엔 없애기
    [Header("플레이어 행동 관리"), SerializeField] protected PlayerActionControl playerActionControl;
    [Header("플레이어 스탯 관리"), SerializeField] protected PlayerStatControl playerStatControl; // 스탯을 관리(버프도 관리)
    [Header("플레이어 움직임 관리"), SerializeField] protected PlayerMovementControl playerMovementControl;

    #region Property
    public PlayerStat PlayerStat { get { return playerStat; }  set { playerStat = value; } }
    public PlayerStatControl GetPlayerStatControl { get { return playerStatControl; } }
    public PlayerMovementControl GetPlayerMovementControl { get { return playerMovementControl; } }
    
    public bool GetIsAlive { get { return isAlive; } }
    public void AnnounceDeath()
    {
        isAlive = false;
        PlayerCtrl playerCtrl = GetComponentInParent<PlayerCtrl>();
        playerCtrl?.DeathChangePlayer();
    }
    #endregion

    #region Override : Set Layer

    /******************************************/
    /************* 레이어 설정  *************/
    /******************************************/

    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    public override bool CanTakeDamageState()  { return playerMovementControl.CanTakeDamage; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { playerStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) {  }

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
        else
            saveStat = new PlayerSaveStat(1,100,1,0,1,1,1);
        
        playerStat = new PlayerStat();
        playerStat.LoadPlayerStat(saveStat);
        playerStatControl.PlayerStat = playerStat;
        playerActionControl.SetPlayerData(playerStatControl, playerMovementControl);
        playerStatusUICtrl = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI;
        playerStatusUICtrl?.Init();

        // 스크립트 연결
        if (playerActionControl == null) playerActionControl = GetComponent<WarriorActionControl>();
        if (playerStatControl == null) playerStatControl = GetComponent<PlayerStatControl>();
        if (playerMovementControl == null) playerMovementControl = GetComponent<WarriorMovementControl>();
        playerMovementControl.Init(playerStat);
    }

    public virtual void Setup()
    {
        playerStatusUICtrl.Setup(playerStat);
        playerMovementControl.Setup();
    }

    public virtual void Execute()
    {
        playerStatusUICtrl.FixedExecute();
        playerMovementControl.Execute();
    }

    public virtual void FixedExecute()
    {
        playerMovementControl.FixedExecute();
    }
    #endregion
}
