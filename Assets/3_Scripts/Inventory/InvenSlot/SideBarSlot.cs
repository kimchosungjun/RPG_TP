using ItemEnums;
using UnityEngine.UI;
using UnityEngine;


public class SideBarSlot : MonoBehaviour
{
    [SerializeField] ITEMTYPE slotType; 
    [SerializeField] Image iconImage;

    public void SetImage(Sprite _sprite)
    {
        iconImage.sprite = _sprite;
    }

    public void PressSideBar()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.ChangeShowItemType(slotType);
    }
}
