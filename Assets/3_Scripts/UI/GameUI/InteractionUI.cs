using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    int currentIndex = 0;
    int activeSlotCnt = 0;
    [SerializeField] List<InteractionSlot> slots;
    
    public void Show()
    {

    }

    public void AddInteractable(Interactable _interactable)
    {
        InteractionSlot slot = GetInActiveSlot();
        if (slot == null) return;
        slot.Active(_interactable);
        activeSlotCnt += 1;
    }

    public InteractionSlot GetInActiveSlot()
    {
        int slotCnt = slots.Count;
        for (int i = 0; i < slotCnt; i++)
        {
            if (slots[i].gameObject.activeSelf == false)
                return slots[i];
        }
        return null;
    }

    public void RemoveInteractable(Interactable _interactable)
    {
        activeSlotCnt -= 1;
        int slotCnt = slots.Count;
        for(int i=0; i<slotCnt; i++)
        {
            slots[i].IsSameData(_interactable);
        }
    }

    #region Input Up & Down Key : Only Use Window Ver
    public void InputUpKey()
    {
        if (currentIndex == 0)
            return;
        slots[currentIndex].InActiveDirection();
        currentIndex -= 1;
        slots[currentIndex].ActiveDirection();
    }

    public void DownKey()
    {
        if (currentIndex == activeSlotCnt)
            return;
        slots[currentIndex].InActiveDirection();
        currentIndex += 1;
        slots[currentIndex].ActiveDirection();
    }
    #endregion
}
