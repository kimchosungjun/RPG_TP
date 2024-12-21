using UnityEngine;

public class WarriorActionControl : PlayerActionControl
{
    [Header("플레이어 행동 데이터")]
    [SerializeField, Tooltip("기본공격")] PlayerNormalAttackActionSOData normalAttackSOData;
    [SerializeField, Tooltip("스킬")] PlayerBuffActionSOData buffActionSOData;
    [SerializeField, Tooltip("궁극기")] PlayerAttackSkillActionSOData ultimateAttackSkillSOData;

    [Header("플레이어 행동")]
    [SerializeField] NearTriggerAttackAction[] normalAttacks;
    
    #region Set Data
    public override void SetPlayerData(PlayerStatControl _statCtrl, PlayerMovementControl _movementControl)
    {
        stat = _statCtrl.PlayerStat;
        movementControl = _movementControl;
        anim = GetComponent<Animator>();    
        PlayerTable playerTable = SharedMgr.TableMgr.Player;

        // 행동의 데이터를 불러오고 그 데이터 지정된 SO에 설정한다.
        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.WARRIOR, stat.GetSaveStat.currentNormalAttackLevel));
        buffActionSOData.SetSOData(playerTable.GetPlayerBuffSkillTableData((int)PlayerEnums.BUFF_SKILLS.WARRIOR_ROAR, stat.GetSaveStat.currentSkillLevel));
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
        TransferAttackData[] attackData = new TransferAttackData[1];
        attackData[0] = new TransferAttackData();
        attackData[0].SetData(normalAttackSOData.GetAttackEffectType(_combo),
            normalAttackSOData.GetActionMultiplier(_combo)*stat.Attack*Randoms.GetCritical(stat.Critical), normalAttackSOData.GetMaintainTime(_combo));
        normalAttacks[_combo].SetTransferData(attackData, null);
    }

    public void StopNormalAttack(int _combo) { normalAttacks[_combo].StopAttack(); }
    #endregion
}
