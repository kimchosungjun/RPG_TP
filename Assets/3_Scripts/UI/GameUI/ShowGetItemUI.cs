using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGetItemUI : UIBase
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

    public override void TurnOn()
    {
        slotGroupParent.SetActive(true);
    }

    public override void TurnOff()
    {
        slotGroupParent.SetActive(false);
    }
    #endregion
}
