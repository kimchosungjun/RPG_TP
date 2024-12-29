using ItemEnums;
using System;
using UnityEngine;
public interface ISlot
{
    public void PressSlot();
}


public class SideBarSlot : MonoBehaviour, ISlot
{
    [SerializeField] ITEMTYPE slotType; 
    public void PressSlot() 
    {
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.ChangeShowItemType(slotType);
    }
}
