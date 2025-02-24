using UnityEngine;

public class WarriorActionControl : PlayerActionControl
{
    [Header("플레이어 행동 데이터")]
    [SerializeField, Tooltip("기본공격")] PlayerNormalAttackActionSOData normalAttackSOData;
    [SerializeField, Tooltip("스킬")] PlayerConditionActionSOData buffActionSOData;
    [SerializeField, Tooltip("궁극기")] PlayerAttackSkillActionSOData ultimateAttackSkillSOData;

    [Header("플레이어 행동")]
    [SerializeField] TriggerAttackAction[] normalAttacks;
    [SerializeField] ProjectileAttackAction farThrowAttacks;
    
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
        buffActionSOData.SetLevelData(stat.GetSaveStat.currentSkillLevel, 3);
        ultimateAttackSkillSOData.SetLevelData(stat.GetSaveStat.currentUltimateSkillLevel, 3);

        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.WARRIOR, stat.GetSaveStat.currentNormalAttackLevel), PlayerEnums.TYPEIDS.WARRIOR);
        buffActionSOData.SetSOData(playerTable.GetPlayerBuffSkillTableData((int)PlayerEnums.CONDITION_SKILLS.WARRIOR_ROAR, stat.GetSaveStat.currentSkillLevel), PlayerEnums.CONDITION_SKILLS.WARRIOR_ROAR);
        ultimateAttackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE, stat.GetSaveStat.currentUltimateSkillLevel), PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE);

        PlayerBaseActionSOData[] actionSoDatas = new PlayerBaseActionSOData[3];
        actionSoDatas[0] = normalAttackSOData;
        actionSoDatas[1] = buffActionSOData;
        actionSoDatas[2] = ultimateAttackSkillSOData;
        _statCtrl.PlayerStat.SetActionSoDatas(actionSoDatas);
    }
    #endregion

    #region Action : Relate Animation
    public void DoNormalAttack(int _combo)
    {
        NormalAttackCoolDown();
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.PLAYER_ATK_SFX);
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(normalAttackSOData.GetAttackEffectType(_combo),
            normalAttackSOData.GetActionMultiplier(_combo)*stat.Attack*Randoms.GetCritical(stat.Critical), normalAttackSOData.GetMaintainTime(_combo));
        normalAttacks[_combo].SetTransferData(attackData, null);
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.WARRIOR_CAST, normalAttacks[_combo].transform.position, normalAttacks[_combo].transform.rotation);
    }

    public void StopNormalAttack(int _combo) 
    { 
        normalAttacks[_combo].StopAttack();
        AnimAttackCooling();
    }

    public void DoBuffParticle()
    {
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.BUFF_SFX);
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ATTACK_BUFF).GetComponent<ParticleAction>().
           SetParticlePosition(transform.position, transform.rotation, 1f);
    }

    public void DoBuffSkill()
    {
        int buffCnt = buffActionSOData.GetBuffCnt();
        SkillCoolDown(SharedMgr.TableMgr.GetPlayer.GetPlayerBuffSkillTableData((int)PlayerEnums.CONDITION_SKILLS.WARRIOR_ROAR, 1).coolTime);
        for (int i = 0; i < buffCnt; i++)
        {
            TransferConditionData transferConditionData = new TransferConditionData();
            transferConditionData.SetData(stat, buffActionSOData.GetEffectStatType(i), buffActionSOData.GetAttributeStatType(i), 
                buffActionSOData.GetContinuityType(i), buffActionSOData.GetDefaultValue(i), 
                buffActionSOData.GetMaintainEffectTime, buffActionSOData.GetMultiplier(i), buffActionSOData.GetApplyType(i));
            statControl.AddCondition(transferConditionData);
        }
    }

    public void DoUltimateAttack()
    {
        UltimateCoolDown(SharedMgr.TableMgr.GetPlayer.GetPlayerAttackSkillTableData((int)PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE, 1).coolTime);
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.PLAYER_ATK_SFX);
        TransferConditionData transferConditionData = new TransferConditionData();
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(ultimateAttackSkillSOData.GetAttackEffectType,
           ultimateAttackSkillSOData.GetActionMultiplier * stat.Attack * Randoms.GetCritical(stat.Critical), ultimateAttackSkillSOData.GetMaintainEffectTime);
        farThrowAttacks.SetTransferData(attackData, null, SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.WARRIOR_SLASH).GetComponent<HitTriggerProjectile>());
    }


    public void DoAnnounceDeathState() 
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.AnnounceDeath();
    }
    #endregion

    #region Common Animation Exit
    public override void EscapeAttackState()
    {
        int cnt = normalAttacks.Length;
        for(int i=0; i<cnt; i++)
        {
            normalAttacks[i].StopAttack();
        }
    }

    public void AnimAttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.AttackCooling(); }
    public void AnimSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.SkillCooling(); }
    public void AnimUltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.UltimateSkillCooling(); }
    public void AnimFallDownCooling() { movementControl.ChangeState(PlayerEnums.STATES.MOVEMENT); }
    #endregion
}
