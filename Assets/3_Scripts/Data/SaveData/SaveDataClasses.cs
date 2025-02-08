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

    public interface ICommonSaveData { public void UpdateData(); }

    [Serializable]
    public class UserSaveDataGroup
    {
        public PlayerSaveDataGroup PlayerSaveDataGroup;
        public InventorySaveDataGroup InventorySaveDataGroup;

        public UserSaveDataGroup() 
        {
            PlayerSaveDataGroup = new PlayerSaveDataGroup();
            InventorySaveDataGroup = new InventorySaveDataGroup();
        }

        public void UpdateAllData()
        {
            PlayerSaveDataGroup.UpdateData();
            InventorySaveDataGroup.UpdateData();
        }
    }

    #region Save Data Group

    [Serializable]
    public class PlayerSaveDataGroup: ICommonSaveData
    {
        #region Values
        // Private Value
        [SerializeField] List<int> cuurrentPlayerPartyIDSet;
        [SerializeField] List<PlayerSaveStat> playerSaveDataSet;
        [SerializeField] Vector3 currentPlayerPosition;
        [SerializeField] Vector3 currentPlayerRotation; 

        // Get Value
        public List<int> CurrentPlayerPartyIDSet { get { return cuurrentPlayerPartyIDSet; } set { cuurrentPlayerPartyIDSet = value; } }   
        public List<PlayerSaveStat> PlayerSaveDataSet { get { return playerSaveDataSet; } set { playerSaveDataSet = value; } }
        public Vector3 CurrentPlayerPosition { get { return currentPlayerPosition; } set { currentPlayerPosition = value; } }   
        public Quaternion GetPlayerRotation() { return Quaternion.Euler(currentPlayerRotation); }
        #endregion

        #region Creator
        public PlayerSaveDataGroup()
        {
            int warriorID = (int)(PlayerEnums.TYPEIDS.WARRIOR);
            cuurrentPlayerPartyIDSet = new List<int>();
            cuurrentPlayerPartyIDSet.Add(warriorID);
            playerSaveDataSet = new List<PlayerSaveStat>();
            PlayerSaveStat saveStat = new PlayerSaveStat(0, 150); 
            playerSaveDataSet.Add(saveStat);    
            currentPlayerPosition = new Vector3(12f, 0f, 290f);
            currentPlayerRotation = new Vector3(0, 180f, 0);
        }
        #endregion

        public void UpdateData()
        {
            Transform playerTransform = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform;
            playerSaveDataSet = SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetAllSaveStat();
            currentPlayerPosition = playerTransform.position;
            currentPlayerRotation = playerTransform.rotation.eulerAngles;
        }
    }

    [Serializable]
    public class InventorySaveDataGroup : ICommonSaveData
    {
        [SerializeField] int gold;
        [SerializeField] List<EtcData> etcSet;
        [SerializeField] List<ConsumeData> consumeSet;
        [SerializeField] List<WeaponData> weaponSet;
       
        public int Gold { get { return gold; } set { gold = value; } }  
        public List<EtcData> EtcSet {  get { return etcSet; } set { etcSet = value; } } 
        public List<ConsumeData> ConsumeSet { get { return consumeSet; } set {  consumeSet = value; } } 
        public List<WeaponData> WeaponSet { get {   return weaponSet; } set { weaponSet = value; } }    
        
        public InventorySaveDataGroup()
        {
            gold = 0;
            etcSet = new List<EtcData>();
            consumeSet = new List<ConsumeData>();   
            weaponSet = new List<WeaponData>();     
         }

        public void LinkData()
        {
            InventoryMgr inven = SharedMgr.InventoryMgr;
            gold = inven.GetGold;
            consumeSet = SharedMgr.InventoryMgr.GetConsumeInventory();
        }

        public void UpdateData()
        {
            InventoryMgr inven = SharedMgr.InventoryMgr;
            etcSet = inven.GetEtcInventory();
            consumeSet = inven.GetConsumeInventory();
            weaponSet = inven.GetWeaponInventory();
        }
    }

   

  

    [Serializable]
    public class QuestConditionSaveData
    {
        [SerializeField] int questID;
        [SerializeField] List<QuestConditionData> conditions = new List<QuestConditionData>();

        public QuestConditionSaveData() { }
        public QuestConditionSaveData(int _questID, List<QuestConditionData> _datas)
        {
            questID = _questID;
            conditions = _datas;
        }
    }
    #endregion

    #region Interaction Data
    [Serializable]
    public class InteractionDataGroup
    {
        public QuestSaveDataGroup saveClearQuest;
        public List<NPCSaveData> npcDataSet;
    
        public NPCSaveData GetNpcSaveData(int _id)
        {
            int cnt = npcDataSet.Count;
            if (cnt == 0)
                return null;

            for(int i=0; i<cnt; i++)
            {
                if (npcDataSet[i].npcID == _id)
                    return npcDataSet[i];
            }

            return null;
        }
    }

    [Serializable]
    public class QuestSaveDataGroup
    {
        public List<int> clearQuestDatas;

        public void AddClearQuest(int _clearQuestID)
        {
            int cnt = clearQuestDatas.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (clearQuestDatas[i] == _clearQuestID)
                    return;
            }
            clearQuestDatas.Add(_clearQuestID);
        }
    }

    [Serializable]
    public class NPCSaveData
    {
        public int npcID;
        public int saveDialogueIndex;
    }
    #endregion
}


