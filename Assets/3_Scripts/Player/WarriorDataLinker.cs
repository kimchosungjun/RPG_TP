using UnityEngine;

public class WarriorDataLinker : PlayerDataLinker
{
    [Header("기본공격"),SerializeField] WarriorNormalAttack[] normalAttacks;
    [SerializeField] PlayerNormalAttackActionSOData normalAttackSOData;

    [Header("일반스킬"),SerializeField] PlayerBuffSkillAction buffSkill;
    [SerializeField] PlayerBuffActionSOData buffActionSOData;

    [Header("궁극기"),SerializeField] WarriorUltimateSkill ultimateSkill;
    [SerializeField] PlayerAttackSkillActionSOData ultimateAttackSkillSOData;

    public override void SetPlayerData(PlayerStatControl _statCtrl)
    {
        stat = _statCtrl.PlayerStat;
        PlayerTable playerTable = SharedMgr.TableMgr.character;

        // 행동의 데이터를 불러오고 그 데이터 지정된 SO에 설정한다.
        normalAttackSOData.SetSOData(playerTable.GetPlayerNormalAttackData((int)PlayerEnums.TYPEIDS.WARRIOR, stat.GetSaveStat.currentNormalAttackLevel));
        buffActionSOData.SetSOData(playerTable.GetPlayerBuffSkillTableData((int)PlayerEnums.BUFF_SKILLS.WARRIOR_ROAR, stat.GetSaveStat.currentSkillLevel));
        ultimateAttackSkillSOData.SetSOData(playerTable.GetPlayerAttackSkillTableData(PlayerEnums.ATTACK_SKILLS.WARRIOR_ULTIMATE, stat.GetSaveStat.currentUltimateSkillLevel));

        int cnt = normalAttacks.Length;
        for (int i=0; i<cnt; i++) { normalAttacks[i].SetStat(stat, i, normalAttackSOData);}
        ultimateSkill.SetStat(stat, ultimateAttackSkillSOData);
        buffSkill.SetStat(stat, buffActionSOData, _statCtrl);
    }
}
