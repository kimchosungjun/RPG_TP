using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//#if UNITY_STANDALONE_WIN
//#if UNITY_EDITOR
//#endif

public class InteractionUI : UIBase
{
    int currentIndex = 0;
    int activeSlotCnt = 0;
    [SerializeField] InteractionSlot[] slots;
    [SerializeField] GameObject slotGroup;
    List<InteractionSlot> activeSlots = new List<InteractionSlot>();

    public void Init()
    {
        SetImages();
    }

    #region Interact
    public void Interaction()
    {
        if (activeSlotCnt == 0) return;
        activeSlots[currentIndex].PressInteractSlot();   
    }

    public void AddInteractable(Interactable _interactable)
    {
        InteractionSlot slot = GetInActiveSlot();
        if (slot == null) return;
#if UNITY_EDITOR
        if (activeSlotCnt == 0) AddChangeDirecionIndex(slot);
#endif
        activeSlots.Add(slot);
         slot.Active(_interactable);

        activeSlotCnt += 1;
    }

    public InteractionSlot GetInActiveSlot()
    {
        int slotCnt = slots.Length;
        for (int i = 0; i < slotCnt; i++)
        {
            if (slots[i].gameObject.activeSelf == false)
                return slots[i];
        }
        return null;
    }

    public void RemoveInteractable(Interactable _interactable)
    {
        int slotCnt = activeSlots.Count;
        for(int i=0; i<slotCnt; i++)
        {
            if (activeSlots[i].IsSameData(_interactable))
            {
                activeSlotCnt -= 1;
                activeSlots.RemoveAt(i);
                RemoveChangeDirectionIndex();
                return;
            }
        }
    }
    #endregion

    #region Input Up & Down Key : Only Use Window Ver
    public bool CanInput()
    {
        if (activeSlotCnt == 0) return false;
        return true;
    }

    public void InputUpKey()
    {
        if (currentIndex == 0)
            return;
        activeSlots[currentIndex].InActiveDirection();
        currentIndex -= 1;
        activeSlots[currentIndex].ActiveDirection();
    }

    public void InputDownKey()
    {
        if (currentIndex == activeSlotCnt - 1)
            return;
        activeSlots[currentIndex].InActiveDirection();
        currentIndex += 1;
        activeSlots[currentIndex].ActiveDirection();
    }

    public void AddChangeDirecionIndex(InteractionSlot _slot)
    {
        currentIndex = 0;
        _slot.ActiveDirection();
    }

    public void RemoveChangeDirectionIndex()
    {
        if (currentIndex >= activeSlotCnt)
        {
            if (currentIndex >= 1)
                currentIndex -= 1;
        }
        if(activeSlotCnt >= 1)
            activeSlots[currentIndex].ActiveDirection();
    }
    #endregion

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface

    public override void TurnOn()
    {
        slotGroup.SetActive(true);
    }

    public override void TurnOff()
    {
        slotGroup.SetActive(false);
    }

    public void SetImages()
    {
        int cnt = slots.Length;
        for(int i=0; i<cnt; i++)
        {
            slots[i].SetImage();
        }
    }
    #endregion
}
