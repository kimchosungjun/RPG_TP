using ItemEnums;
using UnityEngine;

public class BasePlayer : BaseActor
{
    #region Value
    protected int id;
    protected bool isAlive = true;
    protected PlayerStatusUI playerStatusUI; 
    [Header("플레이어 스탯"), SerializeField]protected PlayerStat playerStat; 
    [Header("플레이어 행동 관리"), SerializeField] protected PlayerActionControl playerActionControl;
    [Header("플레이어 스탯 관리"), SerializeField] protected PlayerStatControl playerStatControl; 
    [Header("플레이어 움직임 관리"), SerializeField] protected PlayerMovementControl playerMovementControl;
    [SerializeField] WEAPONTYPE playerWeaponType;
    #endregion

    #region Property
    public PlayerStat PlayerStat { get { return playerStat; }  set { playerStat = value; } }
    public PlayerStatControl GetPlayerStatControl { get { return playerStatControl; } }
    public PlayerMovementControl GetPlayerMovementControl { get { return playerMovementControl; } }
    public WEAPONTYPE GetWeaponType { get { return playerWeaponType; } } 
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }
    public int PlayerID { get { return id; } set { id = value; } }
    // Call After Death Animation 
    public void AnnounceDeath()
    {
        PlayerCtrl playerCtrl = GetComponentInParent<PlayerCtrl>();
        playerCtrl?.DeathChangePlayer();
    }

    // Call Immediately : Show Animation
    public void DoDeathState() { playerMovementControl.Death(); isAlive = false; }
    public void DoRevival() { InitState(); isAlive = true; SetDefaultLayerType(); }
    public void InitState() { playerMovementControl.ChangeState(PlayerEnums.STATES.MOVEMENT); }
    public bool GetCanChangeState { get { return playerMovementControl.CanChangePlayer; } }
    public void SetTransform(Vector3 _position, Quaternion _rotation, Vector3 _velocity)
    {
        
        playerMovementControl.SetMoveRotation = _rotation;
        playerMovementControl.GetRigid.velocity = _velocity;
        transform.position = _position;
        transform.rotation = _rotation;
    }
    #endregion

    /***************************************/
    /************* Set Layer  **************/
    /************* Set Damage  *************/
    /***************************************/

    #region Override  
    public override void SetDefaultLayerType()
    {
        this.gameObject.layer = (int)UtilEnums.LAYERS.PLAYER;
    }

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

    #region Foot Step
    //public void FootStep() { SharedMgr.GameCtrlMgr.GetPlayerCtrl.FootStep(); }
    #endregion

    /****************************************/
    /************ Life Cycle  ***************/
    /****************************************/

    #region Life Cycle : Virtual

    public virtual void Init()
    {
        PlayerSaveStat saveStat = SharedMgr.SaveMgr.GetUserSaveData.PlayerSaveDataGroup.PlayerSaveDataSet[id];
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

        playerMovementControl.Init(playerStat);
    }

    public virtual void Setup()
    {
        playerStatusUI = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI;
        playerMovementControl.Setup();
    }

    public virtual void Execute()
    {
        playerStatControl.FixedExecute();
        playerMovementControl.Execute();
    }

    public virtual void FixedExecute()
    {
        playerMovementControl.FixedExecute();
    }
    #endregion
}
