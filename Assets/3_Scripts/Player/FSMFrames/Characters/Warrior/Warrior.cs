using UnityEngine;

public class Warrior : BasePlayer
{
    [SerializeField] WarriorMoveCtrl warriorMovement;
    [SerializeField] PlayerStatCtrl statCtrl; // 스탯을 관리(버프도 관리)
    PlayerStatusUI playerStatusUICtrl; // HP, EXP, Level을 나타내는 UI

    public override void Init()
    {
        // 스탯 연결
        PlayerSaveStat saveStat = new PlayerSaveStat();
        playerStat = new PlayerStat();
        playerStat.LoadPlayerSaveStat(saveStat);
        playerStat.LoadPlayerStat();
        statCtrl = new PlayerStatCtrl(playerStat);
        playerDataLink.SetPlayerData(statCtrl);
        playerStatusUICtrl = SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUICtrl;
        playerStatusUICtrl?.Init();

        if (warriorMovement==null) warriorMovement = GetComponent<WarriorMoveCtrl>();
        warriorMovement.Init(playerStat);
    }

    public override void Setup()
    {
        playerStatusUICtrl.Setup(playerStat);
        warriorMovement.Setup();
    }

    public override void Execute()
    {
        playerStatusUICtrl.Execute();
        warriorMovement.Execute();
    }

    public override void FixedExecute()
    {
        warriorMovement.FixedExecute();
    }
}
