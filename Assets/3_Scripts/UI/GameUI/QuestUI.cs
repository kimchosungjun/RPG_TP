using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour, IInputKeyUI
{
    [SerializeField, Tooltip("0:QuestIcon, 1: QuestExit")] Image[] questImages;
    [SerializeField] QuestSlot[] questSlots;
    [SerializeField] QuestInfoUI infoUI;
    [SerializeField] GameObject questFrameParent;

    bool isActive = false;
    List<QuestSOData> soDatas = null;
    public void Init()
    {
        SetImages();
        infoUI.Init();
    }

    public void SetImages()
    {
        questImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_3", "Quest_Icon");
        questImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
    
        int questCnt = questSlots.Length;
        for(int i = 0; i < questCnt; i++)
        {
            questSlots[i].Init(i);
        }

        UpdateQuestDatas();
    }

    public void UpdateQuestDatas()
    {
        soDatas = SharedMgr.QuestMgr.GetQuestDatas;
        int questCnt = questSlots.Length;
        int soDataCnt = soDatas.Count;
        for (int i = 0; i < questCnt; i++)
        {
            if (i >= soDataCnt)
                questSlots[i].UpdateSlotData();
            else
                questSlots[i].UpdateSlotData(soDatas[i].GetQuestName);
        }
    }

    public void PressQuestSlot(int _index)
    {
        if (soDatas == null) 
            return;
        int cnt = soDatas.Count;
        if (cnt == 0 || cnt <= _index) 
            return;
        SharedMgr.SoundMgr.PressButtonSFX();
        infoUI.Active(soDatas[_index]);
    }

    #region Relate Active State
    public void InActive()
    {
        infoUI.InActive();
        questFrameParent.SetActive(false);
    }

    public void Active()
    {
        questFrameParent.SetActive(true);
        UpdateQuestDatas();
    }

    public void InputKey()
    {
        isActive = !isActive;
        if (isActive)
        {
            UpdateQuestDatas();
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.QUEST;
        }
        else
        {
            infoUI.InActive();
            SharedMgr.UIMgr.GameUICtrl.CurrentOpenUI = UIEnums.GAMEUI.NONE;
        }
        questFrameParent.SetActive(isActive);
    }

    public void ExitQuestUI()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        InputKey();
    }
    #endregion
}
