using System.Collections;
using UnityEngine;
using PlayerEnums;

public class ArcherMovementControl : PlayerMovementControl
{
    bool canPlayerCtrl = true;
    public bool CanPlayerCtrl { get { return canPlayerCtrl; } }

    #region Unity Life Cycle

    #region Init

    public override void Init(PlayerStat _playerStat)
    {
        base.Init(_playerStat);
        int typeID = _playerStat.GetSaveStat.playerTypeID;
        attackCombo = new PlayerAttackCombo(SharedMgr.TableMgr.GetPlayer.GetPlayerNormalAttackData
            (typeID, _playerStat.GetSaveStat.currentNormalAttackLevel).combo, attackRange);
        LinkMyComponent();
    }

    #endregion

    #region Setup
    public override void Setup()
    {
        SetupValues();
        CreateStates();
    }

    protected override void CreateStates()
    {
        stateMachine = new PlayerStateMachine();
        playerStates = new PlayerState[(int)STATES.MAX];
        playerStates[(int)STATES.MOVEMENT] = new PlayerGroundMoveState(this);
        playerStates[(int)STATES.DASH] = new PlayerDashState(this);
        playerStates[(int)STATES.JUMP] = new ArcherJumpState(this);
        playerStates[(int)STATES.FALL] = new PlayerFallState(this);
        playerStates[(int)STATES.ATTACK] = new PlayerAttackState(this, attackCombo);
        playerStates[(int)STATES.SKILL] = new PlayerSkillState(this, attackCombo);
        playerStates[(int)STATES.ULTIMATESKILL] = new PlayerUltimateSkillState(this, attackCombo);
        playerStates[(int)STATES.HIT] = new PlayerHitState(this);
        playerStates[(int)STATES.DEATH] = new PlayerDeathState(this);
        playerStates[(int)STATES.INTERACTION] = new PlayerInteractionState(this);
        currentPlayerState = STATES.MOVEMENT;
        stateMachine.InitStateMachine(playerStates[(int)STATES.MOVEMENT]);
    }
    #endregion

    #region Execute
    public override void Execute()
    {
        if (canPlayerCtrl)
            stateMachine.Execute();
    }

    public override void FixedExecute()
    {
        stateMachine.FixedExecute();
    }
    #endregion

    #endregion
}
