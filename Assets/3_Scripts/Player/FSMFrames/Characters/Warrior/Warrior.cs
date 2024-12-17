using UnityEngine;

public class Warrior : BasePlayer
{
    PlayerStatusUI playerStatusUICtrl; // HP, EXP, Level을 나타내는 UI

    public override void Init()
    {
        // 스탯 연결
        PlayerSaveStat saveStat = new PlayerSaveStat();
        playerStat = new PlayerStat();
        playerStat.LoadPlayerSaveStat(saveStat);
        playerStat.LoadPlayerStat();
        playerStatControl.PlayerStat = playerStat;
        playerDataLinker.SetPlayerData(playerStatControl);
        playerStatusUICtrl = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUICtrl;
        playerStatusUICtrl?.Init();

        // 스크립트 연결
        if (playerDataLinker == null) playerDataLinker = GetComponent<WarriorDataLinker>();
        if (playerStatControl == null) playerStatControl = GetComponent<PlayerStatControl>();
        if (characterMovementControl == null) characterMovementControl = GetComponent<WarriorMovementControl>();
        characterMovementControl.Init(playerStat);
    }

    public override void Setup()
    {
        playerStatusUICtrl.Setup(playerStat);
        characterMovementControl.Setup();
    }

    public override void Execute()
    {
        playerStatusUICtrl.FixedExecute();
        characterMovementControl.Execute();
    }

    public override void FixedExecute()
    {
        characterMovementControl.FixedExecute();
    }
}
