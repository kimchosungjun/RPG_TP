using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveDataGroup
{
    public interface ICommonSaveData { public void UpdateData(); }

    [Serializable]
    public class UserSaveDataGroup
    {
        public PlayerSaveDataGroup PlayerSaveDataGroup;
        public InventorySaveDataGroup InventorySaveDataGroup;
        public InteractionSaveDataGroup InteractionSaveDataGroup;

        public void UpdateAllData()
        {
            PlayerSaveDataGroup.UpdateData();
            InventorySaveDataGroup.UpdateData();
            InteractionSaveDataGroup.UpdateData();
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
        [SerializeField] List<EtcData> etcSet = new List<EtcData>();
        [SerializeField] List<ConsumeData> consumeSet = new List<ConsumeData>();
        [SerializeField] List<WeaponData> weaponSet = new List<WeaponData>();
       
        public int Gold { get { return gold; } set { gold = value; } }  
        public List<EtcData> EtcSet {  get { return etcSet; } set { etcSet = value; } } 
        public List<ConsumeData> ConsumeSet { get { return consumeSet; } set {  consumeSet = value; } } 
        public List<WeaponData> WeaponSet { get {   return weaponSet; } set { weaponSet = value; } }    
        
        public InventorySaveDataGroup() { gold = 0; }

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


    #endregion

    // Interaction Save Data
    #region Interaction Data Group
    [Serializable]
    public class InteractionSaveDataGroup : ICommonSaveData
    {
        public List<NPCSaveData> npcDataSet = new List<NPCSaveData>();
        public List<QuestLogData> questLogData = new List<QuestLogData> ();
    
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

        public void AddNPCSaveData(NPCSaveData _saveData)
        {
            if (npcDataSet.Contains(_saveData))
                return;
            npcDataSet.Add(_saveData);  
        }

        public QuestLogData GetQuestLogData(int _id)
        {
            int cnt = questLogData.Count;
            if (cnt == 0)
                return null;

            for (int i = 0; i < cnt; i++)
            {
                if (questLogData[i].questID == _id)
                    return questLogData[i];
            }

            return null;
        }

        public void AddQuestLogData(QuestLogData _questData)
        {
            if (questLogData.Contains(_questData))
                return;
            questLogData.Add(_questData);   
        }

        public void UpdateData()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    // Member
    #region Interaction Data Member

    [Serializable]
    public class NPCSaveData
    {
        public int npcID;
        public int npcAcceptQuestID;
        public int saveDialogueID;
        public int saveDialogueIndex;

        public NPCSaveData()
        {
            npcAcceptQuestID = -1;
            saveDialogueID = -1;
            saveDialogueIndex = -1;
        }
    }

    [Serializable]
    public class QuestLogData
    {
        public bool isClearQuest;
        public int questID;
        public List<QuestConditionData> conditions;

        public QuestLogData() { }
        public QuestLogData(bool _isClearQuest, int _questID, List<QuestConditionData> _conditions)
        {
            isClearQuest = _isClearQuest;
            questID = _questID;
            conditions = _conditions;
        }
    }
    #endregion
}

