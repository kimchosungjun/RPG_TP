using UnityEngine;

public class Warrior : BasePlayer
{
    [SerializeField] WarriorMoveCtrl warriorMovement;
    [SerializeField] PlayerStatCtrl statCtrl; // 스탯을 관리(버프도 관리)
    PlayerStatusUICtrl playerStatusUICtrl; // HP, EXP, Level을 나타내는 UI

    public override void Init()
    {
        // 스탯 연결
        PlayerSaveStat saveStat = new PlayerSaveStat();
        playerStat = new PlayerStat();
        playerStat.LoadPlayerSaveStat(saveStat);
        playerStat.LoadPlayerStat();
        statCtrl = new PlayerStatCtrl(playerStat);
        playerDataLink.SetPlayerData(statCtrl);
        
        if(warriorMovement==null) warriorMovement = GetComponent<WarriorMoveCtrl>();
        warriorMovement.Init(playerStat);
    }

    public override void Setup()
    {
        warriorMovement.Setup();
    }

    public override void Execute()
    {
        warriorMovement.Execute();
    }

    public override void FixedExecute()
    {
        warriorMovement.FixedExecute();
    }
}
