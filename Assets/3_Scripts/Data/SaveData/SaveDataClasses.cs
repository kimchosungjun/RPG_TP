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

    // Player Save Data
    #region Player Data Group

    [Serializable]
    public class PlayerSaveDataGroup : ICommonSaveData
    {
        #region Values
        // Private Value
        [SerializeField] List<int> cuurrentPlayerPartyIDSet = new List<int>();
        [SerializeField] List<PlayerSaveStat> playerSaveDataSet = new List<PlayerSaveStat>();
        [SerializeField] Vector3 currentPlayerPosition;
        [SerializeField] Vector3 currentPlayerRotation;

        // Get Value
        public List<int> CurrentPlayerPartyIDSet { get { return cuurrentPlayerPartyIDSet; } }
        public List<PlayerSaveStat> PlayerSaveDataSet { get { return playerSaveDataSet; } }
        public Vector3 CurrentPlayerPosition { get { return currentPlayerPosition; } set { currentPlayerPosition = value; } }
        public Quaternion GetPlayerRotation() { return Quaternion.Euler(currentPlayerRotation); }

        public PlayerSaveStat GetPlayerSaveStat(int _id)
        {
            int cnt = playerSaveDataSet.Count;
            if (cnt == 0) return null;
            for (int i = 0; i < cnt; i++)
            {
                if (playerSaveDataSet[i].playerTypeID == _id)
                    return playerSaveDataSet[i];
            }
            return null;
        }
        #endregion

        public void AddPlayableCharacterID(int _characterID)
        {
            if (cuurrentPlayerPartyIDSet.Contains(_characterID))
                return;
            cuurrentPlayerPartyIDSet.Add(_characterID);
            PlayerSaveStat saveStat = new PlayerSaveStat(_characterID);

            if (playerSaveDataSet.Contains(saveStat))
                return;
            playerSaveDataSet.Add(saveStat);
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.JoinParty(_characterID);
        }

        #region Creator
        public PlayerSaveDataGroup() { }

        public PlayerSaveDataGroup(bool _isFirstCreate)
        {
            if (_isFirstCreate == false)
                return;
            int warriorID = (int)(PlayerEnums.TYPEIDS.WARRIOR);
            cuurrentPlayerPartyIDSet = new List<int>();
            cuurrentPlayerPartyIDSet.Add(warriorID);
            playerSaveDataSet = new List<PlayerSaveStat>();
            PlayerSaveStat saveStat = new PlayerSaveStat(0); 
            playerSaveDataSet.Add(saveStat);    
            currentPlayerPosition = new Vector3(12f, 0f, 290f);
            currentPlayerRotation = new Vector3(0, 180f, 0);
        }
        #endregion

        public void UpdateData()
        {
            Transform playerTransform = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform;
            currentPlayerPosition = playerTransform.position;
            currentPlayerRotation = playerTransform.rotation.eulerAngles;
        }
    }
    #endregion

    // Inventory Save Data
    #region Inventory Save Data Group
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

        public void LinkWeaponUniqueIDSet()
        {
            int weaponSetCnt = weaponSet.Count;
            for(int i=0; i<weaponSetCnt; i++)
            {
                UniqueIDMaker.AddID(weaponSet[i].uniqueID);
            }
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


    #endregion

    // Interaction Save Data
    #region Interaction Data Group
    [Serializable]
    public class InteractionSaveDataGroup : ICommonSaveData
    {
        [SerializeField] List<NPCSaveData> npcDataSet = new List<NPCSaveData>();
        [SerializeField] List<QuestLogData> questLogDataSet = new List<QuestLogData> ();

        #region Manage NPC Data
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
        #endregion

        #region Manage Quest Log Data
        public QuestLogData GetQuestLogData(int _id)
        {
            int cnt = questLogDataSet.Count;
            if (cnt == 0)
                return null;

            for (int i = 0; i < cnt; i++)
            {
                if (questLogDataSet[i].questID == _id)
                    return questLogDataSet[i];
            }
            return null;
        }

        public void AddQuestLogData(QuestLogData _questData)
        {
            if (questLogDataSet.Contains(_questData))
                return;
            questLogDataSet.Add(_questData);   
        }

        public void DeleteQuestLogData(QuestLogData _questData)
        {
            int cnt = questLogDataSet.Count;
            for(int i=0; i<cnt; i++)
            {
                if (questLogDataSet[i] == _questData)
                {
                    questLogDataSet.RemoveAt(i);
                    return;
                }
            }
        }

        public void UpdateData()
        {
            int cnt = questLogDataSet.Count;
            for (int i = 0; i < cnt; i++)
            {
                SharedMgr.QuestMgr.GetQuestData(questLogDataSet[i].questID)?.UpdateLogData();
            }
        }
        #endregion
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
        public int questID;
        public List<QuestConditionData> conditions;

        public QuestLogData() { }
        public QuestLogData(int _questID, List<QuestConditionData> _conditions)
        {
            questID = _questID;
            conditions = _conditions;
        }
    }
    #endregion
}

