using UnityEngine;

public abstract class BasePlayer : BaseActor
{
    /*[SerializeField] */protected PlayerStat playerStat; // 확인을 위해 serialize로 설정 : 나중엔 없애기

    [Header("플레이어 스탯 연결"), SerializeField] protected PlayerActionControl playerDataLinker;
    [Header("플레이어 스탯 컨트롤"),SerializeField] protected PlayerStatControl playerStatControl; // 스탯을 관리(버프도 관리)
    [Header("플레이어 움직임 제어"), SerializeField] protected PlayerMovementControl characterMovementControl;
    
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

    #region Abstract Method
    public abstract void Init();
    public abstract void Setup();
    public abstract void Execute();
    public abstract void FixedExecute();
    #endregion
}
