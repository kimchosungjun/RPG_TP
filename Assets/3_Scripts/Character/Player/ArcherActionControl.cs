
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

    public override void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl)
    {
        this.statControl = _statCtrl;
        stat = statControl.PlayerStat;
        movementControl = _movementControl;
        anim = GetComponent<Animator>();
        PlayerTable playerTable = SharedMgr.TableMgr.GetPlayer;

        // 행동의 데이터를 불러오고 그 데이터 지정된 SO에 설정한다.
        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.ARCHER, stat.GetSaveStat.currentNormalAttackLevel));
        attackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.ARCHER_FULL_BLOWN_SHOOT, stat.GetSaveStat.currentSkillLevel));
        ultimateAttackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.ARCHER_ULTIMATE, stat.GetSaveStat.currentUltimateSkillLevel));
    }


    #region Animations 
    public void AnimAttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.AttackCooling(); }
    public void AnimSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.SkillCooling(); }
    public void AnimUltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.UltimateSkillCooling(); }
    public void AnimDashCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.DashCooling(); }

    public void DoNormalAttack(int _combo)
    {
        // Set Data
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(normalAttackSOData.GetAttackEffectType(_combo),
            normalAttackSOData.GetActionMultiplier(_combo) * stat.Attack * Randoms.GetCritical(stat.Critical), normalAttackSOData.GetMaintainTime(_combo));
        normalAttacks[_combo].SetTransferData
            (attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_NORMAL).GetComponent<HitThrowBox>());

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
            (attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_NORMAL).GetComponent<HitThrowBox>());

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
        ultimateAttack.SetTransferData(attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_ULTIMATE).GetComponent<HitThrowBox>());

        // Particle
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ARCHER_POP).GetComponent<ParticleAction>().
            SetParticlePosition(popTransform.position, popTransform.rotation, 1f);
    }

    public void DoAnnounceDeathState()
    {
        SharedMgr.EnvironmentMgr.GetPlayerCtrl.GetPlayer.AnnounceDeath();
    }
    #endregion
}
