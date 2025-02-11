using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InteractionMgr 
{
    InteractionNPC currentInteractNpc = null;
    public InteractionNPC CurrentInteractNPC { get { return currentInteractNpc; } set { currentInteractNpc = value; } }

    bool isConversation = false;
    public bool GetIsConversation { get { return isConversation; } }

    #region Dialogue Data Value
    Dictionary<int, Dialogue> dialogueGroup = new Dictionary<int, Dialogue>();
    DialogueReader reader = new DialogueReader();
    public DialogueReader GetDialogueReader { get { return reader; } }
    #endregion

    #region Load & Get Dialouge Data
    public void LoadDialogues(string _path)
    {
        TextAsset[] textAssets = SharedMgr.ResourceMgr.LoadAllResource<TextAsset>(_path);
        int textCnt = textAssets.Length;    
        for(int i=0; i<textCnt; i++)
        {
            DialogueDataSet data = reader.ConvertToJsonData(textAssets[i].text);
            int dialogueCnt = data.dialogueSet.Count;
            for (int k = 0; k < dialogueCnt; k++)
            {
                if (dialogueGroup.ContainsKey(data.dialogueSet[k].dialogueID))
                    continue;
                dialogueGroup.Add(data.dialogueSet[k].dialogueID, data.dialogueSet[k]);
            }
        }
    }

    public Dialogue GetDialouge(int _dialougeID)
    {
        if (dialogueGroup.ContainsKey(_dialougeID) == false)
            return null;
        return dialogueGroup[_dialougeID];
    }
    #endregion

    #region Conversation

    public void StartConversation(InteractionNPC _npc)
    {
        if (isConversation)
            return;
        Dialogue data = GetDialouge(_npc.DialogueID);
        if (data == null)
            return;
        isConversation = true;
        currentInteractNpc = _npc;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.StartConversation(data);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetTalkCamerView(currentInteractNpc.transform);
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.StartConversation(currentInteractNpc.transform.position);
        SharedMgr.UIMgr.GameUICtrl.StartConversation();
    }

    public void StartConversation(InteractionNPC _npc, int _dialougeIndex)
    {
        if (isConversation) 
            return;
        Dialogue data = GetDialouge(_npc.DialogueID);
        if (data == null)
            return;
        isConversation = true;
        currentInteractNpc = _npc;
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.StartConversation(data, _dialougeIndex);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetTalkCamerView(currentInteractNpc.transform);
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.StartConversation(currentInteractNpc.transform.position);
        SharedMgr.UIMgr.GameUICtrl.StartConversation();
    }

    public void ContinueConversation(int _nextDialogueIndex)
    {
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.ContinueConversation(_nextDialogueIndex);

    }

    public void EndConversation()
    {
        SharedMgr.UIMgr.GameUICtrl.EndConversation();
        SharedMgr.UIMgr.GameUICtrl.GetDialogueUI.EndConversation();
        SharedMgr.GameCtrlMgr.GetCameraCtrl.ReSetTalkCameraView();
    }

    public void ReleaseMoveLock()
    {
        isConversation = false;
        currentInteractNpc = null;
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.EndConversation();
    }

    #endregion
}
