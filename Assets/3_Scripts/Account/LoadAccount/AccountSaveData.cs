using System.Collections.Generic;
using System;

[Serializable]
public class AccountSaveData 
{
    public List<PlayerSaveStat> playerSaveStatList;
    public List<int> playerPartyList;

    /// <summary>
    /// Enter Game : Call This Method
    /// </summary>
    public void GetPlayerParty()
    {
        if(playerSaveStatList==null)
        {
            playerSaveStatList = new List<PlayerSaveStat>();
            playerPartyList = new List<int>();
            PlayerTableClassGroup.PlayerTableData playerTableData =
                SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(PlayerEnums.TYPEIDS.WARRIOR);
            PlayerSaveStat playerSaveStat = new PlayerSaveStat(playerTableData.id, playerTableData.defaultHP);

            playerSaveStatList.Add(playerSaveStat);
            playerPartyList.Add(playerSaveStat.playerTypeID);
            return;
        }

        SharedMgr.TableMgr.AccountSaveData = this;
    }
}
