
using UnityEngine;

public class ArcherActionControl : PlayerActionControl
{
    [Header("플레이어 행동 데이터")]
    [SerializeField, Tooltip("기본공격")] PlayerNormalAttackActionSOData normalAttackSOData;
    [SerializeField, Tooltip("스킬")] PlayerAttackSkillActionSOData attackSkillSOData;
    [SerializeField, Tooltip("궁극기")] PlayerAttackSkillActionSOData ultimateAttackSkillSOData;

    [SerializeField] ProjectileAttackAction[] normalAttacks;
    [SerializeField] ProjectileAttackAction[] skillAttacks;
    [SerializeField] ProjectileAttackAction ultimateAttack;
    [SerializeField] Transform popTransform;

    #region Set Data
    public override void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl)
    {
        this.statControl = _statCtrl;
        stat = statControl.PlayerStat;
        movementControl = _movementControl;
        anim = GetComponent<Animator>();
        PlayerTable playerTable = SharedMgr.TableMgr.GetPlayer;

        // 행동의 데이터를 불러오고 그 데이터 지정된 SO에 설정한다.
        normalAttackSOData.SetLevelData(stat.GetSaveStat.currentNormalAttackLevel, 3);
        attackSkillSOData.SetLevelData(stat.GetSaveStat.currentSkillLevel, 3);
        ultimateAttackSkillSOData.SetLevelData(stat.GetSaveStat.currentUltimateSkillLevel, 3);

        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.ARCHER, stat.GetSaveStat.currentNormalAttackLevel), PlayerEnums.TYPEIDS.ARCHER);
        attackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.ARCHER_FULL_BLOWN_SHOOT, stat.GetSaveStat.currentSkillLevel), PlayerEnums.ATTACK_SKILLS.ARCHER_FULL_BLOWN_SHOOT);
        ultimateAttackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.ARCHER_ULTIMATE, stat.GetSaveStat.currentUltimateSkillLevel), PlayerEnums.ATTACK_SKILLS.ARCHER_ULTIMATE);

        PlayerBaseActionSOData[] actionSoDatas = new PlayerBaseActionSOData[3];
        actionSoDatas[0] = normalAttackSOData;
        actionSoDatas[1] = attackSkillSOData;
        actionSoDatas[2] = ultimateAttackSkillSOData;
        _statCtrl.PlayerStat.SetActionSoDatas(actionSoDatas);
    }
    #endregion

    #region  Action : Relate Animation

    public void DoNormalAttack(int _combo)
    {
        // Set Data
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(normalAttackSOData.GetAttackEffectType(_combo),
            normalAttackSOData.GetActionMultiplier(_combo) * stat.Attack * Randoms.GetCritical(stat.Critical), normalAttackSOData.GetMaintainTime(_combo));
        normalAttacks[_combo].SetTransferData
            (attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_NORMAL).GetComponent<HitTriggerProjectile>());

        // Particle
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_POP).GetComponent<ParticleAction>().
            SetParticlePosition(popTransform.position, popTransform.rotation, 1f);
    }

    public void DoSkillAttack(int _combo)
    {
        // Set Data
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(attackSkillSOData.GetAttackEffectType,
            attackSkillSOData.GetActionMultiplier * stat.Attack * Randoms.GetCritical(stat.Critical), attackSkillSOData.GetMaintainEffectTime);
        skillAttacks[_combo].SetTransferData
            (attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_NORMAL).GetComponent<HitTriggerProjectile>());

        // Particle
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_POP).GetComponent<ParticleAction>().
            SetParticlePosition(popTransform.position, popTransform.rotation, 1f);
    }

    public void DoUltimateAttack()
    {
        TransferConditionData transferConditionData = new TransferConditionData();
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(ultimateAttackSkillSOData.GetAttackEffectType,
           ultimateAttackSkillSOData.GetActionMultiplier * stat.Attack * Randoms.GetCritical(stat.Critical), ultimateAttackSkillSOData.GetMaintainEffectTime);
        ultimateAttack.SetTransferData(attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_ULTIMATE).GetComponent<HitTriggerProjectile>());

        // Particle
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_POP).GetComponent<ParticleAction>().
            SetParticlePosition(popTransform.position, popTransform.rotation, 1f);
    }

    public void DoAnnounceDeathState()
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.AnnounceDeath();
    }
    #endregion

    #region Common Animation Exit
    public void AnimAttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.AttackCooling(); }
    public void AnimSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.SkillCooling(); }
    public void AnimUltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.UltimateSkillCooling(); }
    public void AnimFallDownCooling() { movementControl.ChangeState(PlayerEnums.STATES.MOVEMENT); }
    //public void AnimDashCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.DashCooling(); }
    #endregion
}
