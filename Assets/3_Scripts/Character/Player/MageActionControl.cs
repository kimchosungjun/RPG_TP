using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageActionControl : PlayerActionControl
{
    [Header("플레이어 행동 데이터")]
    [SerializeField, Tooltip("기본공격")] PlayerNormalAttackActionSOData normalAttackSOData;
    [SerializeField, Tooltip("스킬")] PlayerConditionActionSOData buffActionSOData;
    [SerializeField, Tooltip("궁극기")] PlayerAttackSkillActionSOData ultimateAttackSkillSOData;

    [Header("플레이어 행동")]
    [SerializeField] TriggerAttackAction[] normalAttacks;

    [SerializeField] Transform[] normalAttackParticleTransforms;
    [SerializeField] Transform ultimateMeteorSpellTransform;
    [SerializeField] Transform meteorTransform;
    [SerializeField] Transform ultimateExplosionTransofrm;

    [SerializeField, Tooltip("플레이어 파티 버프 관리")] PartyConditionControl partyConditionControl;

    public PartyConditionControl GetPartyConditionControl()
    {
        if(partyConditionControl != null) return partyConditionControl; 
        partyConditionControl = GetComponentInParent<PartyConditionControl>();
        return partyConditionControl;
    }
    #region Set Data
    public override void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl)
    {
        this.statControl = _statCtrl;
        stat = statControl.PlayerStat;
        movementControl = _movementControl;
        anim = GetComponent<Animator>();
        PlayerTable playerTable = SharedMgr.TableMgr.GetPlayer;

        // 행동의 데이터를 불러오고 그 데이터 지정된 SO에 설정한다.
        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.MAGE, stat.GetSaveStat.currentNormalAttackLevel));
        buffActionSOData.SetSOData(playerTable.GetPlayerBuffSkillTableData(PlayerEnums.CONDITION_SKILLS.MAGE_VITALITY_INCREASE, stat.GetSaveStat.currentSkillLevel));
        ultimateAttackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE, stat.GetSaveStat.currentUltimateSkillLevel));
    }
    #endregion

    #region Animations 
    public void AnimAttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.AttackCooling(); }
    public void AnimSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.SkillCooling(); }
    public void AnimUltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.UltimateSkillCooling(); }
    public void AnimDashCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); movementControl.DashCooling(); }

    public void DoNormalAttack(int _combo)
    {
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(normalAttackSOData.GetAttackEffectType(_combo),
            normalAttackSOData.GetActionMultiplier(_combo) * stat.Attack * Randoms.GetCritical(stat.Critical), normalAttackSOData.GetMaintainTime(_combo));
        normalAttacks[_combo].SetTransferData(attackData, null);
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.MAGICIAN_NORMAL).GetComponent<ParticleAction>().
            SetParticlePosition(normalAttackParticleTransforms[_combo].transform.position, normalAttackParticleTransforms[_combo].transform.rotation, 1.5f);
    }

    public void StopNormalAttack(int _combo)
    {
        normalAttacks[_combo].StopAttack();
        AnimAttackCooling();
    }

    public void DoBuffSkill()
    {
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.ATTACK_BUFF).GetComponent<ParticleAction>().
           SetParticlePosition(transform.position, transform.rotation, 1f);
        int buffCnt = buffActionSOData.GetBuffCnt();
        for (int i = 0; i < buffCnt; i++)
        {
            TransferConditionData transferConditionData = new TransferConditionData();

            if((int)EffectEnums.CONDITION_PARTY.NONE == buffActionSOData.GetPartyType(i))
            {
                // Add Player Own Stat Control
                transferConditionData.SetData(stat, buffActionSOData.GetEffectStatType(i), buffActionSOData.GetAttributeStatType(i),
    buffActionSOData.GetContinuityType(i), buffActionSOData.GetDefaultValue(i),
    buffActionSOData.GetMaintainEffectTime, buffActionSOData.GetMultiplier(i), buffActionSOData.GetApplyType(i));
                statControl.AddCondition(transferConditionData);
            }
            else
            {
                // Add Party Stat Control
                transferConditionData.SetData(stat, buffActionSOData.GetEffectStatType(i), buffActionSOData.GetAttributeStatType(i),
buffActionSOData.GetContinuityType(i), buffActionSOData.GetDefaultValue(i),
buffActionSOData.GetMaintainEffectTime, buffActionSOData.GetMultiplier(i), buffActionSOData.GetApplyType(i));
                GetPartyConditionControl()?.AddCondition(transferConditionData);
            }     
        }
    }

    public void DoUltimateAttack()
    {
        TransferAttackData attackData = new TransferAttackData();
        attackData.SetData(ultimateAttackSkillSOData.GetAttackEffectType,
           ultimateAttackSkillSOData.GetActionMultiplier * stat.Attack * Randoms.GetCritical(stat.Critical), ultimateAttackSkillSOData.GetMaintainEffectTime);
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.MAGICIAN_METEOR).GetComponent<CreateExplosion>
            ().SetTransferData(attackData, null, meteorTransform.position, meteorTransform.rotation, Vector3.down, 
            ultimateExplosionTransofrm.position, ultimateExplosionTransofrm.rotation, 2f, PoolEnums.OBJECTS.EXPLOSION);
    }

    public void DoAnnounceDeathState()
    {
        SharedMgr.EnvironmentMgr.GetPlayerCtrl.GetPlayer.AnnounceDeath();
    }

    public void MeteorSpell()
    {
        SharedMgr.PoolMgr.GetPool(PoolEnums.OBJECTS.MAGIC_CIRCLE_SPELL).GetComponent<ParticleAction>().
          SetParticlePosition(ultimateMeteorSpellTransform.transform.position, ultimateMeteorSpellTransform.transform.rotation, 1.5f);
    }
    #endregion
}
