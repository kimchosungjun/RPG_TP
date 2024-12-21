using UnityEngine;

public class BasePlayer : BaseActor
{
    [Header("플레이어 상태 UI"), SerializeField]protected PlayerStatusUI playerStatusUICtrl; // HP, EXP, Level을 나타내는 UI
    [Header("플레이어 스탯"), SerializeField]protected PlayerStat playerStat; // 확인을 위해 serialize로 설정 : 나중엔 없애기
    [Header("플레이어 행동 관리"), SerializeField] protected PlayerActionControl playerActionControl;
    [Header("플레이어 스탯 관리"), SerializeField] protected PlayerStatControl playerStatControl; // 스탯을 관리(버프도 관리)
    [Header("플레이어 움직임 관리"), SerializeField] protected PlayerMovementControl characterMovementControl;

    #region Property
    public PlayerStat PlayerStat { get { return playerStat; }  set { playerStat = value; } }
    public PlayerStatControl GetPlayerStatControl { get { return playerStatControl; } }
    public PlayerMovementControl GetCharacterMovementControl { get { return characterMovementControl; } }
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

    public override bool CanTakeDamageState()  { return characterMovementControl.CanTakeDamage; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { playerStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) {  }

    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Life Cycle : Virtual
    public virtual void Init()
    {
        // 스탯 연결
        PlayerSaveStat saveStat = new PlayerSaveStat();
        playerStat = new PlayerStat();
        playerStat.LoadPlayerStat(saveStat);
        playerStatControl.PlayerStat = playerStat;
        playerActionControl.SetPlayerData(playerStatControl, characterMovementControl);
        playerStatusUICtrl = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUICtrl;
        playerStatusUICtrl?.Init();

        // 스크립트 연결
        if (playerActionControl == null) playerActionControl = GetComponent<WarriorActionControl>();
        if (playerStatControl == null) playerStatControl = GetComponent<PlayerStatControl>();
        if (characterMovementControl == null) characterMovementControl = GetComponent<WarriorMovementControl>();
        characterMovementControl.Init(playerStat);
    }

    public virtual void Setup()
    {
        playerStatusUICtrl.Setup(playerStat);
        characterMovementControl.Setup();
    }

    public virtual void Execute()
    {
        playerStatusUICtrl.FixedExecute();
        characterMovementControl.Execute();
    }

    public virtual void FixedExecute()
    {
        characterMovementControl.FixedExecute();
    }
    #endregion
}
