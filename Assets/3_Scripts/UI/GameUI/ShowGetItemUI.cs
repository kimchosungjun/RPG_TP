using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGetItemUI : MonoBehaviour, ICommonSetUI
{
    [SerializeField] ShowGetItemSlot[] slots;
    [SerializeField] GameObject slotGroupParent;

    public void Init()
    {
        SetImages();
    }
  
    public ShowGetItemSlot[] GetSlots() 
    {
        return slots; 
    }


    /******************************************/
    /**************  Interface  ***************/
    /******************************************/
    
    #region Interface
    public void SetImages()
    {
        for(int i = 0; i < 2; i++)
        {
            slots[i].SetImages();
        }
    }

    public void TurnOn()
    {
        slotGroupParent.SetActive(true);
    }

    public void TurnOff()
    {
        slotGroupParent.SetActive(false);
    }
    #endregion
}
