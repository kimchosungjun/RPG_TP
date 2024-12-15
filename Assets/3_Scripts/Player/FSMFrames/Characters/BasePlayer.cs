using UnityEngine;

public abstract class BasePlayer : BaseCharacter
{
    /*[SerializeField] */protected PlayerStat playerStat; // 확인을 위해 serialize로 설정 : 나중엔 없애기

    [Header("플레이어 스탯 연결"), SerializeField] protected PlayerDataLinker playerDataLinker;
    [Header("플레이어 스탯 컨트롤"),SerializeField] protected PlayerStatControl playerStatControl; // 스탯을 관리(버프도 관리)
    [Header("플레이어 움직임 제어"), SerializeField] protected CharacterMovementControl characterMovementControl;
    
    #region Property
    public PlayerStat PlayerStat { get { return playerStat; }  set { playerStat = value; } }
    public PlayerStatControl GetPlayerStatControl { get { return playerStatControl; } }
    public CharacterMovementControl GetCharacterMovementControl { get { return characterMovementControl; } }
    #endregion

    #region Override : Set Layer
    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }
    #endregion

    #region Abstract Method
    public abstract void Init();
    public abstract void Setup();
    public abstract void Execute();
    public abstract void FixedExecute();
    #endregion
}
