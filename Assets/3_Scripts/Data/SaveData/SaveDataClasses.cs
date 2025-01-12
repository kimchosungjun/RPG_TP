using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 깃 안에 깃을 만들어서 예외처리

namespace SaveDataGroup
{
    #region Origin
    public class SaveDataClasses { }
    #endregion

    [Serializable]
    public class UserSaveData
    {
        public PlayerSaveDataGroup PlayerSaveDataGroup;
        public InventorySaveDataGroup InventorySaveDataGroup;
    }

    #region Save Data Group

    [Serializable]
    public class PlayerSaveDataGroup
    {
        [SerializeField] List<int> cuurrentPlayerPartyIDSet;
        [SerializeField] List<PlayerSaveStat> playerSaveDataSet;
        [SerializeField] Vector3 currentPlayerPosition;

        public List<int> CurrentPlayerPartyIDSet { get { return cuurrentPlayerPartyIDSet; } set { cuurrentPlayerPartyIDSet = value; } }   
        public List<PlayerSaveStat> PlayerSaveDataSet { get { return playerSaveDataSet; } set { playerSaveDataSet = value; } }
        public Vector3 CurrentPlayerPosition { get { return currentPlayerPosition; } set { currentPlayerPosition = value; } }   

        public PlayerSaveDataGroup()
        {
            int warriorID = (int)(PlayerEnums.TYPEIDS.WARRIOR);
            cuurrentPlayerPartyIDSet = new List<int>();
            cuurrentPlayerPartyIDSet.Add(warriorID);
            playerSaveDataSet = new List<PlayerSaveStat>();
            PlayerSaveStat saveStat = new PlayerSaveStat(warriorID,SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(warriorID).defaultHP); 
            playerSaveDataSet.Add(saveStat);    
            currentPlayerPosition = Vector3.zero;
        }
    }

    [Serializable]
    public class InventorySaveDataGroup
    {
        [SerializeField] int gold;
        [SerializeField] List<EtcData> etcSet;
        [SerializeField] List<ConsumeData> consumeSet;
        [SerializeField] List<WeaponData> weaponSet;
        [SerializeField] List<WeaponData> holdWeaponSet;

        public int Gold { get { return gold; } set { gold = value; } }  
        public List<EtcData> EtcSet {  get { return etcSet; } set { etcSet = value; } } 
        public List<ConsumeData> ConsumeSet { get { return consumeSet; } set {  consumeSet = value; } } 
        public List<WeaponData> WeaponSet { get {   return weaponSet; } set { weaponSet = value; } }    
        public List<WeaponData> HoldWeaponSet { get { return holdWeaponSet; } set { holdWeaponSet = value; } }       

        public InventorySaveDataGroup()
        {
            gold = 0;
            etcSet = new List<EtcData>();
            consumeSet = new List<ConsumeData>();   
            weaponSet = new List<WeaponData>();     
            holdWeaponSet = new List<WeaponData>(); 
        }

        public void LinkData()
        {
            InventoryMgr inven = SharedMgr.InventoryMgr;
            gold = inven.GetGold;
            
            consumeSet = SharedMgr.InventoryMgr.GetConsumeInventory();
        }
    }

    public class QuestSaveDataGroup
    {

    }

    public class NPCSaveDataGroup
    {
        public int dialogueIndex;
        public int currentQuestIndex = -1;
        public List<int> clearDialogues;
    }
    #endregion
}


